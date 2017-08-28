using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface MapConfiguration : Serializable
	{
		Map GenerateMap();
	}
}
