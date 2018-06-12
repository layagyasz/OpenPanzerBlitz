using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface MapConfiguration : Serializable
	{
		MapConfiguration MakeStatic(Random Random);
		Map GenerateMap(Random Random, Environment Environment, IdGenerator IdGenerator);
	}
}
