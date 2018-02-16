using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitIsHostile : Matcher<Unit>
	{
		enum Attribute { TEAM };

		public readonly byte Team;

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

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Team);
		}

		public bool Matches(Unit Unit)
		{
			return Unit.Army.Configuration.Team != Team;
		}

		public IEnumerable<Matcher<Unit>> Flatten()
		{
			yield return this;
		}
	}
}
