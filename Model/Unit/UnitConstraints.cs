using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConstraints : Serializable
	{
		enum Attribute { INTRODUCE_YEAR, OBSOLETE_YEAR, FRONT, ENVIRONMENTS };

		public readonly int IntroduceYear;
		public readonly int ObsoleteYear;
		public readonly Front Front;
		public readonly List<Environment> Environments;

		public UnitConstraints()
		{
			IntroduceYear = 0;
			ObsoleteYear = 0;
			Front = Front.ALL;
			Environments = new List<Environment>();
		}

		public UnitConstraints(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			IntroduceYear = (int)(attributes[(int)Attribute.INTRODUCE_YEAR] ?? 0);
			ObsoleteYear = (int)(attributes[(int)Attribute.OBSOLETE_YEAR] ?? 0);
			Front = (Front)(attributes[(int)Attribute.FRONT] ?? Front.ALL);
			Environments = (List<Environment>)(attributes[(int)Attribute.ENVIRONMENTS] ?? new List<Environment>());
		}

		public UnitConstraints(SerializationInputStream Stream)
		{
			IntroduceYear = Stream.ReadInt32();
			ObsoleteYear = Stream.ReadInt32();
			Front = (Front)Stream.ReadByte();
			Environments = Stream.ReadEnumerable(
				i => Stream.ReadObject(j => new Environment(j), false, true)).ToList();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(IntroduceYear);
			Stream.Write(ObsoleteYear);
			Stream.Write((byte)Front);
			Stream.Write(Environments, i => Stream.Write(i, false, true));
		}
	}
}
