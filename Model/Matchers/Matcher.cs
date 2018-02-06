using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface Matcher<T> : Serializable
	{
		bool Matches(T Object);
		IEnumerable<Matcher<T>> Flatten();
	}
}
