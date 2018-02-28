using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasClass : Matcher<Unit>
	{
		enum Attribute { UNIT_CLASS };

		public readonly UnitClass UnitClass;

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

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)UnitClass);
		}

		public bool Matches(Unit Unit)
		{
			return Unit.Configuration.UnitClass == UnitClass;
		}

		public IEnumerable<Matcher<Unit>> Flatten()
		{
			yield return this;
		}
	}
}
