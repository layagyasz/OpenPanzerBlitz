using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class BlankMapConfiguration : MapConfiguration
	{
		public readonly int Width;
		public readonly int Height;

		public BlankMapConfiguration(int Width, int Height)
		{
			this.Width = Width;
			this.Height = Height;
		}

		public BlankMapConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadInt32(), Stream.ReadInt32()) { }

		public MapConfiguration MakeStatic(Random Random)
		{
			return this;
		}

		public Map GenerateMap(Random Random, Environment Environment, IdGenerator IdGenerator)
		{
			var map = new Map(Width, Height, Environment, IdGenerator);
			map.Ready();
			return map;
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Width);
			Stream.Write(Height);
		}
	}
}
