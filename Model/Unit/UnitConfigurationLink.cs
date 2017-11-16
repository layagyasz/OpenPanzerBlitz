using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfigurationLink
	{
		enum Attribute { FACTION, UNIT_CONFIGURATION };

		public readonly Faction Faction;
		public readonly UnitConfiguration UnitConfiguration;

		public UnitConfigurationLink(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Faction = (Faction)attributes[(int)Attribute.FACTION];
			UnitConfiguration = (UnitConfiguration)attributes[(int)Attribute.UNIT_CONFIGURATION];
		}
	}
}
