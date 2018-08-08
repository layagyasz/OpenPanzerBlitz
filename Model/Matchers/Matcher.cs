using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public abstract class Matcher<T> : Serializable
	{
		public abstract bool IsTransient { get; }
		public abstract bool Matches(T Object);
		public abstract void Serialize(SerializationOutputStream Stream);

		public bool MatchesTransient(T Object)
		{
			return IsTransient || Matches(Object);
		}

		public virtual IEnumerable<Matcher<T>> Flatten()
		{
			yield return this;
		}
	}
}
