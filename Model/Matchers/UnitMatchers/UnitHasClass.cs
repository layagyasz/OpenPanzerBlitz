using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasClass : Matcher<Unit>
	{
		enum Attribute { UNIT_CLASS };

		public readonly UnitClass UnitClass;

		public override bool IsTransient { get; } = false;

		public UnitHasClass(UnitClass UnitClass)
		{
			this.UnitClass = UnitClass;
		}

		public UnitHasClass(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitClass = (UnitClass)attributes[(int)Attribute.UNIT_CLASS];
		}

		public UnitHasClass(SerializationInputStream Stream)
			: this((UnitClass)Stream.ReadByte()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)UnitClass);
		}

		public override bool Matches(Unit Object)
		{
			if (Object == null) return false;
			return Object.Configuration.UnitClass == UnitClass;
		}
	}
}
