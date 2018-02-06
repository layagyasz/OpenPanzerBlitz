using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class EmptyMatcher<T> : Matcher<T>
	{
		public EmptyMatcher() { }

		public EmptyMatcher(ParseBlock Block) { }

		public EmptyMatcher(SerializationInputStream Stream) { }

		public bool Matches(T Object)
		{
			return true;
		}

		public IEnumerable<Matcher<T>> Flatten()
		{
			yield return this;
		}

		public void Serialize(SerializationOutputStream Stream) { }
	}
}
