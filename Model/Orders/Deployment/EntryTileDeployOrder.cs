using System;
using System.Collections.Generic;

using Cardamom.Serialization;

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

		public EntryTileDeployOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((ConvoyDeployment)Objects[Stream.ReadInt32()], (Tile)Objects[Stream.ReadInt32()]) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Deployment.Id);
			Stream.Write(Tile.Id);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.DEPLOYMENT;
		}

		public override OrderInvalidReason Validate()
		{
			return Deployment.Validate(Tile);
		}

		public override OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Deployment.SetEntryTile(Tile);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}

		public override string ToString()
		{
			return string.Format("[EntryTileDeployOrder: Tile={0}]", Tile);
		}
	}
}
