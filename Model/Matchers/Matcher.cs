using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface Matcher<T> : Serializable
	{
		bool Matches(T Object);
	}
}
