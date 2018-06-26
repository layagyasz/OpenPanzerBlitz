using System.Collections.Generic;
using System.Linq;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class ConvoyDeploymentHandler
	{
		public readonly AIRoot Root;

		public ConvoyDeploymentHandler(AIRoot Root)
		{
			this.Root = Root;
		}

		public IEnumerable<Order> Handle(ConvoyDeployment Deployment)
		{
			var order = Deployment.Units
								  .Where(i => !Root.UnitAssignments.GetAssignments(i).Any(
									  j => j.Object == i && j.AssignmentType == UnitAssignmentType.CARRIER))
								  .ToList();
			order.Sort(new FluentComparator<Unit>(i => i.Configuration.IsVehicle)
					   .ThenCompare(i => i.Configuration.IsArmored)
					   .ThenCompare(i => i.Configuration.UnitClass != UnitClass.TRANSPORT)
					   .ThenCompare(i => i.Configuration.Movement)
					   .Invert());

			foreach (var assignment in order.SelectMany(
				i => Root.UnitAssignments.GetAssignments(i)).Where(i => i.AssignmentType == UnitAssignmentType.CARRIER))
				yield return new LoadOrder(assignment.Subject, assignment.Object, false);
			yield return new ConvoyOrderDeployOrder(Deployment, order);

			var entryTiles = Root.Match.GetMap().TilesEnumerable.Where(
				i => i.Configuration.HasPathOverlay(TilePathOverlay.ROAD)
					&& Deployment.Validate(i) == OrderInvalidReason.NONE).ToList();
			yield return new EntryTileDeployOrder(
				Deployment,
				entryTiles.ArgMax(
					i => Root.Random.NextDouble() - (.9 + Root.Random.NextDouble() * .1)
					* order.Sum(j => Root.TileEvaluations.GetThreatRating(i, j))));
		}
	}
}
