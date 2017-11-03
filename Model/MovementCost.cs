using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public struct MovementCost
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
	}
}
