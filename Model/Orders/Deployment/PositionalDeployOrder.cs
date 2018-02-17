using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class PositionalDeployOrder : DeployOrder
	{
		public readonly Unit Unit;
		public readonly Tile Tile;

		public override Army Army
		{
			get
			{
				return Unit.Army;
			}
		}

		public PositionalDeployOrder(Unit Unit, Tile Tile)
		{
			this.Unit = Unit;
			this.Tile = Tile;
		}

		public PositionalDeployOrder(SerializationInputStream Stream, List<GameObject> Objects)
		{
			Unit = (Unit)Objects[Stream.ReadInt32()];
			if (Stream.ReadBoolean()) Tile = (Tile)Objects[Stream.ReadInt32()];
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Unit.Id);
			Stream.Write(Tile != null);
			if (Tile != null) Stream.Write(Tile.Id);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.DEPLOYMENT;
		}

		public override OrderInvalidReason Validate()
		{
			if (Unit.Deployment is PositionalDeployment)
				return ((PositionalDeployment)Unit.Deployment).Validate(Unit, Tile);
			return OrderInvalidReason.DEPLOYMENT_RULE;
		}

		public override OrderStatus Execute(Random Random)
		{
			if (Validate() != OrderInvalidReason.NONE) return OrderStatus.ILLEGAL;

			if (Tile == null)
			{
				if (Unit.Position != null) Unit.Position.ClearControl(Unit);
				if (Unit.Carrier != null) Unit.Carrier.Unload(false);
				if (Unit.Passenger != null) Unit.Unload(false);
				Unit.Remove();
				Unit.Emplace(false);
			}
			else
			{
				Tile.Control(Unit);
				Unit.Place(Tile);
				Unit.Emplace(true);
			}

			return OrderStatus.FINISHED;
		}
	}
}
