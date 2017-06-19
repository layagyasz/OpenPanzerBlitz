using System;
using System.Linq;

namespace PanzerBlitz
{
	public class DeployOrder : Order
	{
		public readonly Unit Unit;
		public readonly Tile Tile;

		public DeployOrder(Unit Unit, Tile Tile)
		{
			this.Unit = Unit;
			this.Tile = Tile;
		}

		public NoDeployReason Validate()
		{
			return Unit.Army.Deployments.Find(i => i.Units.Contains(Unit)).Validate(Unit, Tile);
		}

		public bool Execute(Random Random)
		{
			if (Validate() != NoDeployReason.NONE) return false;

			if (Tile == null) Unit.Remove();
			else Unit.Place(Tile);

			return true;
		}
	}
}
