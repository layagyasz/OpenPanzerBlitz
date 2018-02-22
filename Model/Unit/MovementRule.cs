using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public struct MovementRule : Serializable
	{
		public readonly BlockType BlockType;
		public readonly float Cost;

		public MovementRule(BlockType BlockType)
		{
			this.BlockType = BlockType;
			Cost = 0;
		}

		public MovementRule(float Cost)
		{
			BlockType = BlockType.NONE;
			this.Cost = Cost;
		}

		public MovementRule(SerializationInputStream Stream)
		{
			if (Stream.ReadBoolean())
			{
				Cost = Stream.ReadFloat();
				BlockType = BlockType.NONE;
			}
			else
			{
				Cost = 0;
				BlockType = (BlockType)Stream.ReadByte();
			}
		}

		public MovementRule(ParseBlock Block)
		{
			try
			{
				Cost = Convert.ToSingle(Block.String, System.Globalization.CultureInfo.InvariantCulture);
				BlockType = BlockType.NONE;
			}
			catch
			{
				BlockType = Parse.EnumParser<BlockType>(typeof(BlockType))(Block.String);
				Cost = 0;
			}
		}

		public MovementCost GetMoveCost(bool Adjacent, bool UnitMoved)
		{
			if (BlockType == BlockType.NONE) return new MovementCost(Cost);
			return GetBlockTypeMoveCost(BlockType, Adjacent, UnitMoved);
		}

		MovementCost GetBlockTypeMoveCost(BlockType Block, bool Adjacent, bool UnitMoved)
		{
			switch (Block)
			{
				case BlockType.NONE: return new MovementCost(0f);
				case BlockType.HARD_BLOCK:
					return new MovementCost(
						Adjacent
							 ? (UnitMoved ? OrderInvalidReason.MOVEMENT_TERRAIN : 0f)
							 : OrderInvalidReason.MOVEMENT_TERRAIN);
				case BlockType.IMPASSABLE: return new MovementCost(OrderInvalidReason.MOVEMENT_TERRAIN);
				case BlockType.SOFT_BLOCK: return new MovementCost(UnitMoved ? OrderInvalidReason.MOVEMENT_TERRAIN : 0);
				case BlockType.STANDARD: return new MovementCost(0f);
				default: throw new Exception(string.Format("No movement cost for {0}", BlockType));
			}
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Cost > 0);
			if (Cost > 0) Stream.Write(Cost);
			else Stream.Write((byte)BlockType);
		}
	}
}
