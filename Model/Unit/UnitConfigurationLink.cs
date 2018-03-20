using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfigurationLink : Serializable
	{
		enum Attribute { FACTION, UNIT_CONFIGURATION, INTRODUCE_YEAR, OBSOLETE_YEAR, FRONT, ENVIRONMENTS };

		public readonly Faction Faction;
		public readonly UnitConfiguration UnitConfiguration;
		public readonly int IntroduceYear;
		public readonly int ObsoleteYear;
		public readonly Front Front;
		public readonly List<Environment> Environments;

		public UnitConfigurationLink(SerializationInputStream Stream)
		{
			Faction = Stream.ReadObject(i => new Faction(i), false, true);
			UnitConfiguration = Stream.ReadObject(i => new UnitConfiguration(i), false, true);
			IntroduceYear = Stream.ReadInt32();
			ObsoleteYear = Stream.ReadInt32();
			Front = (Front)Stream.ReadByte();
			if (Stream.ReadBoolean())
				Environments = Stream.ReadEnumerable(
					i => Stream.ReadObject(j => new Environment(j), false, true)).ToList();
		}

		public UnitConfigurationLink(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Faction = (Faction)attributes[(int)Attribute.FACTION];
			UnitConfiguration = (UnitConfiguration)attributes[(int)Attribute.UNIT_CONFIGURATION];
			IntroduceYear = (int)(attributes[(int)Attribute.INTRODUCE_YEAR] ?? 0);
			ObsoleteYear = (int)(attributes[(int)Attribute.OBSOLETE_YEAR] ?? 0);
			Front = (Front)(attributes[(int)Attribute.FRONT] ?? Front.ALL);
			Environments = (List<Environment>)attributes[(int)Attribute.ENVIRONMENTS];
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Faction, false, true);
			Stream.Write(UnitConfiguration, false, true);
			Stream.Write(IntroduceYear);
			Stream.Write(ObsoleteYear);
			Stream.Write((byte)Front);
			Stream.Write(Environments != null);
			if (Environments != null) Stream.Write(Environments, i => Stream.Write(i, false, true));
		}
	}
}
