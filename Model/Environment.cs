using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Environment
	{
		enum Attribute { TILE_RULE_SET, MOVEMENT_MULTIPLIER };

		public readonly string UniqueKey;
		public readonly TileRuleSet TileRuleSet;
		public readonly float MovementMultiplier;

		public Environment(string UniqueKey, TileRuleSet TileRuleSet, float MovementMultiplier)
		{
			this.UniqueKey = UniqueKey;
			this.TileRuleSet = TileRuleSet;
			this.MovementMultiplier = MovementMultiplier;
		}

		public Environment(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			TileRuleSet = (TileRuleSet)attributes[(int)Attribute.TILE_RULE_SET];
			MovementMultiplier = Parse.DefaultIfNull(attributes[(int)Attribute.MOVEMENT_MULTIPLIER], 1f);
		}
	}
}
