using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitIsHostile : Matcher<Unit>
	{
		enum Attribute { TEAM };

		public readonly byte Team;

		public override bool IsTransient { get; } = false;

		public UnitIsHostile(byte Team)
		{
			this.Team = Team;
		}

		public UnitIsHostile(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Team = (byte)attributes[(int)Attribute.TEAM];
		}

		public UnitIsHostile(SerializationInputStream Stream)
			: this(Stream.ReadByte()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Team);
		}

		public override bool Matches(Unit Object)
		{
			return Object.Army.Configuration.Team != Team;
		}
	}
}
