using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfigurationLink : Serializable
	{
		enum Attribute { FACTION, UNIT_CONFIGURATION, CONSTRAINTS };

		public readonly string UniqueKey;
		public readonly Faction Faction;
		public readonly UnitConfiguration UnitConfiguration;
		public readonly UnitConstraints Constraints;

		public UnitConfigurationLink(SerializationInputStream Stream)
		{
			UniqueKey = Stream.ReadString();
			Faction = Stream.ReadObject(i => new Faction(i), false, true);
			UnitConfiguration = Stream.ReadObject(i => new UnitConfiguration(i), false, true);
			Constraints = new UnitConstraints(Stream);
		}

		public UnitConfigurationLink(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			Faction = (Faction)attributes[(int)Attribute.FACTION];
			UnitConfiguration = (UnitConfiguration)attributes[(int)Attribute.UNIT_CONFIGURATION];
			Constraints = (UnitConstraints)(attributes[(int)Attribute.CONSTRAINTS] ?? new UnitConstraints());
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(Faction, false, true);
			Stream.Write(UnitConfiguration, false, true);
			Stream.Write(Constraints);
		}
	}
}
