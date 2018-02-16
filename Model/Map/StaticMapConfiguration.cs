using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class StaticMapConfiguration : MapConfiguration
	{
		public readonly Map Map;

		public StaticMapConfiguration(Map Map)
		{
			this.Map = Map;
		}

		public StaticMapConfiguration(SerializationInputStream Stream)
			: this(new Map(Stream, null, new IdGenerator())) { }

		public Map GenerateMap(Environment Environment, IdGenerator IdGenerator)
		{
			var map = new Map(Map.Width, Map.Height, Environment, IdGenerator);
			map.CopyTo(Map, 0, 0, false);
			return map;
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Map.Serialize(Stream);
		}
	}
}
