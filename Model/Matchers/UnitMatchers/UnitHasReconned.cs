using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasReconned : Matcher<Unit>
	{
		enum Attribute { DIRECTION };

		public readonly Direction Direction;

		public UnitHasReconned(Direction Direction)
		{
			this.Direction = Direction;
		}

		public UnitHasReconned(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Direction = (Direction)attributes[(int)Attribute.DIRECTION];
		}

		public UnitHasReconned(SerializationInputStream Stream)
					: this((Direction)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Direction);
		}

		public bool Matches(Unit Unit)
		{
			return Unit.HasRecon(Direction);
		}

		public IEnumerable<Matcher<Unit>> Flatten()
		{
			yield return this;
		}
	}
}
