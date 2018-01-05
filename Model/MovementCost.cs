using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public struct MovementCost : Serializable
	{
		public readonly BlockType BlockType;
		public readonly float Cost;

		public MovementCost(BlockType BlockType)
		{
			this.BlockType = BlockType;
			this.Cost = 0;
		}

		public MovementCost(float Cost)
		{
			this.BlockType = BlockType.NONE;
			this.Cost = Cost;
		}

		public MovementCost(SerializationInputStream Stream)
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

		public MovementCost(ParseBlock Block)
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

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Cost > 0);
			if (Cost > 0) Stream.Write(Cost);
			else Stream.Write((byte)BlockType);
		}
	}
}
