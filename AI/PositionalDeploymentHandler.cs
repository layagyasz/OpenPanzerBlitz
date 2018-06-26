using System.Collections.Generic;
using System.Linq;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class PositionalDeploymentHandler
	{
		public readonly AIRoot Root;

		public PositionalDeploymentHandler(AIRoot Root)
		{
			this.Root = Root;
		}

		public IEnumerable<Order> Handle(PositionalDeployment Deployment)
		{
			var deployments = new Dictionary<Unit, Tile>();

			var seeds = Deployment.Units.Where(
				i => i.Position == null && Root.UnitAssignments.GetAssignments(i).All(j => j.Subject != i)).ToList();
			seeds.Sort(new FluentComparator<Unit>(i => i.Configuration.UnitClass == UnitClass.COMMAND_POST)
					   .ThenCompare(i => i.Configuration.SpotRange)
					   .Invert());
			var coveredTiles = new HashSet<Tile>();
			foreach (var unit in seeds)
			{
				var tile = Root.Match.GetMap().TilesEnumerable
								.Where(i => Deployment.Validate(unit, i) == OrderInvalidReason.NONE)
								.ArgMax(i => ScoreSeedTile(unit, i, coveredTiles));
				deployments.Add(unit, tile);
				foreach (var t in unit.GetFieldOfSight(AttackMethod.DIRECT_FIRE, tile))
					coveredTiles.Add(t.Final);
				yield return new PositionalDeployOrder(unit, tile);
			}

			var defenders =
				Deployment.Units
						  .SelectMany(i => Root.UnitAssignments.GetAssignments(i).Where(
							  j => j.Subject == i && j.AssignmentType == UnitAssignmentType.DEFENDER))
						  .ToList();
			foreach (var assignment in defenders)
			{
				var tile = Root.Match.GetMap().TilesEnumerable
								.Where(i => Deployment.Validate(assignment.Subject, i) == OrderInvalidReason.NONE)
								.ArgMax(i => ScoreDefenseTile(assignment, i, deployments));
				deployments.Add(assignment.Subject, tile);
				yield return new PositionalDeployOrder(assignment.Subject, tile);
			}

			var carriers =
				Deployment.Units
						  .SelectMany(i => Root.UnitAssignments.GetAssignments(i).Where(
							  j => j.Subject == i && j.AssignmentType == UnitAssignmentType.CARRIER))
							.ToList();
			foreach (var assignment in carriers)
			{
				var tile = Root.Match.GetMap().TilesEnumerable
								.Where(i => Deployment.Validate(assignment.Subject, i) == OrderInvalidReason.NONE)
								.ArgMax(i => ScoreCarrierTile(assignment, i, deployments));
				deployments.Add(assignment.Subject, tile);
				yield return new PositionalDeployOrder(assignment.Subject, tile);
			}
		}

		double BaseTileScore(Tile Tile)
		{
			return (Tile.Units.Any(i => i.Configuration.UnitClass == UnitClass.FORT) ? 8 : 1)
				* (Tile.Rules.Concealing ? 4 : 1);
		}

		double ScoreSeedTile(Unit Unit, Tile Tile, HashSet<Tile> CoveredTiles)
		{
			var tiles =
				Unit.GetFieldOfSight(
					Unit.Configuration.SpotRange > 0
						? Unit.Configuration.SpotRange : 20, Tile, AttackMethod.DIRECT_FIRE)
					.Select(i => i.Final)
					.ToList();
			return BaseTileScore(Tile) * (tiles.Count + tiles.Except(CoveredTiles).Count());
		}

		double ScoreDefenseTile(UnitAssignment Assignment, Tile Tile, Dictionary<Unit, Tile> Deployments)
		{
			return BaseTileScore(Tile)
				/ (1 + Tile.HexCoordinate.Distance(Deployments[Assignment.Object].HexCoordinate));
		}

		double ScoreCarrierTile(UnitAssignment Assignment, Tile Tile, Dictionary<Unit, Tile> Deployments)
		{
			return BaseTileScore(Tile) * Assignment.Subject.Configuration.Movement
				/ (1 + Tile.HexCoordinate.Distance(Deployments[Assignment.Object].HexCoordinate));
		}
	}
}
