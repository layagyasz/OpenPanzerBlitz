using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class EmptyMatcher<T> : Matcher<T>
	{
		public override bool IsTransient { get; } = false;

		public EmptyMatcher() { }

		public EmptyMatcher(ParseBlock Block) { }

		public EmptyMatcher(SerializationInputStream Stream) { }

		public override bool Matches(T Object)
		{
			return true;
		}

		public override void Serialize(SerializationOutputStream Stream) { }
	}
}
