using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class BoardCompositeMapConfiguration : MapConfiguration
	{
		public readonly List<List<Tuple<string, bool>>> Boards;

		public BoardCompositeMapConfiguration(IEnumerable<IEnumerable<Tuple<string, bool>>> Boards)
		{
			this.Boards = Boards.Select(i => i.ToList()).ToList();
		}

		public BoardCompositeMapConfiguration(ParseBlock Block)
		{
			Boards = Block.BreakToList<List<Tuple<object, object>>>().Select(
				i => i.Select(
					j => new Tuple<string, bool>((string)j.Item1, (bool)j.Item2)).ToList()).ToList();
		}

		public BoardCompositeMapConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadEnumerable(
				i => Stream.ReadEnumerable(
					j => new Tuple<string, bool>(Stream.ReadString(), Stream.ReadBoolean()))))
		{ }

		public Map GenerateMap()
		{
			List<List<Tuple<Map, bool>>> boards = Boards.Select(i => i.Select(j =>
			{
				FileStream f = FileUtils.GetStream(j.Item1, FileMode.Open, 1000);
				Map m = new Map(new SerializationInputStream(new GZipStream(f, CompressionMode.Decompress)));
				f.Close();
				return new Tuple<Map, bool>(m, j.Item2);
			}).ToList()).ToList();

			int width = boards.Max(i => i.Sum(j => j.Item1.Width) - i.Count + 1);
			int height = boards.Sum(i => i.Max(j => j.Item1.Height)) - Boards.Count + 1;

			Map map = new Map(width, height);

			int rowY = 0;
			foreach (List<Tuple<Map, bool>> mapRow in boards)
			{
				int rowX = 0;
				int nextRowY = 0;
				foreach (Tuple<Map, bool> m in mapRow)
				{
					map.CopyTo(m.Item1.Tiles, rowX, rowY, m.Item2);
					rowX += m.Item1.Width - 1;
					nextRowY = Math.Max(nextRowY, rowY + m.Item1.Height - 1);
				}
				rowY = nextRowY;
			}

			map.Ready();
			return map;
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Boards, i => Stream.Write(i, j =>
			{
				Stream.Write(j.Item1);
				Stream.Write(j.Item2);
			}));
		}
	}
}
