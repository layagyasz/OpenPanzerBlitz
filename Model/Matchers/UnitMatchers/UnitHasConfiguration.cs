using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasConfiguration : Matcher<Unit>
	{
		enum Attribute { UNIT_CONFIGURATION };

		public readonly UnitConfiguration UnitConfiguration;

		public override bool IsTransient { get; } = false;

		public UnitHasConfiguration(UnitConfiguration UnitConfiguration)
		{
			this.UnitConfiguration = UnitConfiguration;
		}

		public UnitHasConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitConfiguration = (UnitConfiguration)attributes[(int)Attribute.UNIT_CONFIGURATION];
		}

		public UnitHasConfiguration(SerializationInputStream Stream)
			: this(GameData.UnitConfigurations[Stream.ReadString()]) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitConfiguration.UniqueKey);
		}

		public override bool Matches(Unit Object)
		{
			if (Object == null) return false;
			return Object.Configuration == UnitConfiguration;
		}
	}
}
