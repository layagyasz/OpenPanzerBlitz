using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class TileEvaluator
	{
		public readonly AIRoot Root;

		Dictionary<Tile, Tuple<double, double>> _ThreatRatings = new Dictionary<Tile, Tuple<double, double>>();

		public TileEvaluator(AIRoot Root)
		{
			this.Root = Root;
		}

		public void ReEvaluate()
		{
			_ThreatRatings.Clear();
			foreach (Unit unit in Root.Match.GetArmies()
					 .Where(i => i.Configuration.Team != Root.Army.Configuration.Team)
					 .SelectMany(i => i.Units))
			{
				foreach (var los in unit.GetFieldOfSight(AttackMethod.DIRECT_FIRE))
				{
					AddThreat(los.Final, GetThreat(unit, los, true), GetThreat(unit, los, false));
				}
			}
		}

		public double GetThreatRating(Tile Tile, Unit Unit)
		{
			if (_ThreatRatings.ContainsKey(Tile))
			{
				Tuple<double, double> threat = _ThreatRatings[Tile];
				return Unit.GetPointValue() * (Unit.Configuration.IsArmored ? threat.Item1 : threat.Item2)
						   / Unit.Configuration.Defense;
			}
			return 0;
		}

		public double GetPotentialRating(Tile Tile, Unit Unit)
		{
			double potential = 0;
			foreach (Unit unit in Root.Match.GetArmies()
					 .Where(i => i.Configuration.Team != Root.Army.Configuration.Team)
					 .SelectMany(i => i.Units)
					 .Where(i => i.Position.HexCoordinate.Distance(Tile.HexCoordinate)
							<= Unit.Configuration.GetRange(AttackMethod.DIRECT_FIRE, false)))
			{
				potential += unit.GetPointValue() * GetPotential(Unit, new LineOfSight(Tile, unit.Position))
								 / unit.Configuration.Defense;
			}
			return potential;
		}

		public double GetTileFavorability(Tile Tile, Unit Unit)
		{
			return GetPotentialRating(Tile, Unit) / Math.Max(1, GetThreatRating(Tile, Unit));
		}

		void AddThreat(Tile Tile, double Armored, double UnArmored)
		{
			if (_ThreatRatings.ContainsKey(Tile))
			{
				Tuple<double, double> threat = _ThreatRatings[Tile];
				_ThreatRatings[Tile] = new Tuple<double, double>(threat.Item1 + Armored, threat.Item2 + UnArmored);
			}
			else _ThreatRatings.Add(Tile, new Tuple<double, double>(Armored, UnArmored));
		}

		double GetThreat(Unit Unit, LineOfSight LineOfSight, bool EnemyArmored)
		{
			return Math.Max(
				new AttackFactorCalculation(Unit, AttackMethod.DIRECT_FIRE, EnemyArmored, LineOfSight, true).Attack,
				new AttackFactorCalculation(Unit, AttackMethod.DIRECT_FIRE, EnemyArmored, LineOfSight, false).Attack);
		}

		double GetPotential(Unit Unit, LineOfSight LineOfSight)
		{
			var defenders =
				LineOfSight.Final.Units.Where(
					i => i.CanBeAttackedBy(Root.Army, AttackMethod.DIRECT_FIRE, true) == OrderInvalidReason.NONE);
			var armoredCount = defenders.Count(i => i.Configuration.IsArmored);
			var unArmoredCount = defenders.Count(i => !i.Configuration.IsArmored);
			if (armoredCount > unArmoredCount
				|| defenders.Any(i => i.Configuration.UnitClass == UnitClass.FORT)
				|| LineOfSight.Final.Rules.TreatUnitsAsArmored)
				return GetThreat(Unit, LineOfSight, true);
			if (armoredCount < unArmoredCount) return GetThreat(Unit, LineOfSight, false);
			return Math.Min(GetThreat(Unit, LineOfSight, true), GetThreat(Unit, LineOfSight, false));
		}
	}
}
