using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfigurationLink
	{
		enum Attribute { FACTION, UNIT_CONFIGURATION, INTRODUCE_YEAR, OBSOLETE_YEAR, FRONT };

		public readonly Faction Faction;
		public readonly UnitConfiguration UnitConfiguration;
		public readonly int IntroduceYear;
		public readonly int ObsoleteYear;
		public readonly Front Front;

		public UnitConfigurationLink(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Faction = (Faction)attributes[(int)Attribute.FACTION];
			UnitConfiguration = (UnitConfiguration)attributes[(int)Attribute.UNIT_CONFIGURATION];
			IntroduceYear = Parse.DefaultIfNull(attributes[(int)Attribute.INTRODUCE_YEAR], 0);
			ObsoleteYear = Parse.DefaultIfNull(attributes[(int)Attribute.OBSOLETE_YEAR], 0);
			Front = Parse.DefaultIfNull(attributes[(int)Attribute.FRONT], Front.ALL);
		}
	}
}
