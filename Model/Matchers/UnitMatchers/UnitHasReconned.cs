using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasReconned : Matcher<Unit>
	{
		enum Attribute { DIRECTION };

		public readonly Direction Direction;

		public override bool IsTransient { get; } = true;

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

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Direction);
		}

		public override bool Matches(Unit Object)
		{
			if (Object == null) return false;
			return Object.HasRecon(Direction);
		}
	}
}
