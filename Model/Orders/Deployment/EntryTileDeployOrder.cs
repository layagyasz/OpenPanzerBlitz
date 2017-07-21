using System;
namespace PanzerBlitz
{
	public class EntryTileDeployOrder : DeployOrder
	{
		public readonly ConvoyDeployment Deployment;
		public readonly Tile Tile;

		public override Army Army
		{
			get
			{
				return Deployment.Army;
			}
		}

		public EntryTileDeployOrder(ConvoyDeployment Deployment, Tile Tile)
		{
			this.Deployment = Deployment;
			this.Tile = Tile;
		}

		public override NoDeployReason Validate()
		{
			return Deployment.Validate(Tile);
		}

		public override bool Execute(Random Random)
		{
			if (Validate() == NoDeployReason.NONE)
			{
				Deployment.SetEntryTile(Tile);
				return true;
			}
			return false;
		}
	}
}
