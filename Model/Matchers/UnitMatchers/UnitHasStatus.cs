using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasStatus : Matcher<Unit>
	{
		enum Attribute { STATUS };

		public readonly UnitStatus Status;

		public UnitHasStatus(UnitStatus Status)
		{
			this.Status = Status;
		}

		public UnitHasStatus(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Status = (UnitStatus)attributes[(int)Attribute.STATUS];
		}

		public UnitHasStatus(SerializationInputStream Stream)
			: this((UnitStatus)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Status);
		}

		public bool Matches(Unit Unit)
		{
			return Unit.Status == Status;
		}

		public IEnumerable<Matcher<Unit>> Flatten()
		{
			yield return this;
		}
	}
}
