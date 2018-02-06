using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasEvacuated : Matcher<Unit>
	{
		enum Attribute { DIRECTION };

		public readonly Direction Direction;

		public UnitHasEvacuated(Direction Direction)
		{
			this.Direction = Direction;
		}

		public UnitHasEvacuated(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Direction = (Direction)attributes[(int)Attribute.DIRECTION];
		}

		public UnitHasEvacuated(SerializationInputStream Stream)
							: this((Direction)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Direction);
		}

		public bool Matches(Unit Unit)
		{
			return Unit.Evacuated == Direction;
		}

		public IEnumerable<Matcher<Unit>> Flatten()
		{
			yield return this;
		}
	}
}
