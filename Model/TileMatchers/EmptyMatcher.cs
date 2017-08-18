using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class EmptyMatcher : Matcher
	{
		public EmptyMatcher() { }

		public EmptyMatcher(SerializationInputStream Stream) { }

		public bool Matches(Tile Tile)
		{
			return true;
		}

		public void Serialize(SerializationOutputStream Stream) { }
	}
}
