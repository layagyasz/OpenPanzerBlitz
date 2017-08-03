using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface Matcher : Serializable
	{
		bool Matches(Tile Tile);
	}
}
