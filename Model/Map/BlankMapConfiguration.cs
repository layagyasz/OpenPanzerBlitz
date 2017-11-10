using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class BlankMapConfiguration : MapConfiguration
	{
		int _Width;
		int _Height;

		public BlankMapConfiguration(int Width, int Height)
		{
			_Width = Width;
			_Height = Height;
		}

		public BlankMapConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadInt32(), Stream.ReadInt32()) { }

		public Map GenerateMap(Environment Environment, IdGenerator IdGenerator)
		{
			Map map = new Map(_Width, _Height, Environment, IdGenerator);
			map.Ready();
			return map;		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_Width);
			Stream.Write(_Height);
		}
	}
}
