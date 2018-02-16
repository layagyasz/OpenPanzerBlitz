using System;
using System.IO;
using System.IO.Compression;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class BoardConfiguration : Serializable
	{
		enum Attribute { BOARD_PATH, INVERT };

		public readonly string BoardPath;
		public readonly bool Invert;

		public BoardConfiguration(string BoardPath, bool Invert)
		{
			this.BoardPath = BoardPath;
			this.Invert = Invert;
		}

		public BoardConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			BoardPath = (string)attributes[(int)Attribute.BOARD_PATH];
			Invert = (bool)attributes[(int)Attribute.INVERT];
		}

		public BoardConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadString(), Stream.ReadBoolean()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(BoardPath);
			Stream.Write(Invert);
		}

		public Tuple<Map, bool> LoadMap()
		{
			var f = FileUtils.GetStream(BoardPath, FileMode.Open, 1000);
			var m = new Map(
				new SerializationInputStream(
					new GZipStream(f, CompressionMode.Decompress)), null, new IdGenerator());
			f.Close();
			return new Tuple<Map, bool>(m, Invert);
		}
	}
}
