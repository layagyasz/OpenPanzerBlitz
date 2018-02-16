using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfigurationLock
	{
		enum Attribute { UNIT_CONFIGURATION, FACTION, TAGS, RARITY }

		public readonly int Id;
		public readonly UnitConfiguration UnitConfiguration;
		public readonly Faction Faction;
		public readonly string[] Tags;
		public readonly int Rarity;

		public UnitConfigurationLock(ParseBlock Block, IdGenerator IdGenerator)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Id = IdGenerator.GenerateId();
			UnitConfiguration = GameData.UnitConfigurations[(string)attributes[(int)Attribute.UNIT_CONFIGURATION]];
			Faction = GameData.Factions[(string)attributes[(int)Attribute.FACTION]];
			Tags = (string[])attributes[(int)Attribute.TAGS];
			Rarity = (int)attributes[(int)Attribute.RARITY];
		}
	}
}
