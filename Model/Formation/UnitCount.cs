using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitCount : Serializable
	{
		enum Attribute { UNIT_CONFIGURATION, COUNT };

		public readonly UnitConfiguration UnitConfiguration;
		public readonly int Count;

		public UnitCount(UnitConfiguration UnitConfiguration, int Count)
		{
			this.UnitConfiguration = UnitConfiguration;
			this.Count = Count;
		}

		public UnitCount(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitConfiguration = (UnitConfiguration)attributes[(int)Attribute.UNIT_CONFIGURATION];
			Count = (int)attributes[(int)Attribute.COUNT];
		}

		public UnitCount(SerializationInputStream Stream)
			: this(GameData.UnitConfigurations[Stream.ReadString()], Stream.ReadInt32()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitConfiguration.UniqueKey);
			Stream.Write(Count);
		}

		public override string ToString()
		{
			return string.Format("[UnitCount: Count={0}, UnitConfiguration={1}]", Count, UnitConfiguration.UniqueKey);
		}
	}
}
