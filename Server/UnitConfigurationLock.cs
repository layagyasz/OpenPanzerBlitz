using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfigurationLock
	{
		enum Attribute { UNIT_CONFIGURATIONS, FACTION, RARITY }

		public readonly int Id;
		public readonly List<UnitConfiguration> UnitConfigurations;
		public readonly Faction Faction;
		public readonly float Rarity;

		public UnitConfigurationLock(ParseBlock Block, IdGenerator IdGenerator)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Id = IdGenerator.GenerateId();
			UnitConfigurations = ((List<string>)attributes[(int)Attribute.UNIT_CONFIGURATIONS])
				.Select(i => GameData.UnitConfigurations[i]).ToList();
			Faction = GameData.Factions[(string)attributes[(int)Attribute.FACTION]];
			Rarity = (float)attributes[(int)Attribute.RARITY];
		}
	}
}
