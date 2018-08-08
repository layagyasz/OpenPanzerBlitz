using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasStatus : Matcher<Unit>
	{
		enum Attribute { STATUS };

		public readonly UnitStatus Status;

		public override bool IsTransient { get; } = true;

		public UnitHasStatus(UnitStatus Status)
		{
			this.Status = Status;
		}

		public UnitHasStatus(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Status = (UnitStatus)attributes[(int)Attribute.STATUS];
		}

		public UnitHasStatus(SerializationInputStream Stream)
			: this((UnitStatus)Stream.ReadByte()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Status);
		}

		public override bool Matches(Unit Object)
		{
			return Object.Status == Status;
		}
	}
}
