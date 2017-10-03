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
		public readonly List<List<BoardConfiguration>> Boards;

		public BoardCompositeMapConfiguration(IEnumerable<IEnumerable<BoardConfiguration>> Boards)
		{
			this.Boards = Boards.Select(i => i.ToList()).ToList();
		}

		public BoardCompositeMapConfiguration(ParseBlock Block)
		{
			Boards = Block.BreakToList<List<BoardConfiguration>>();
		}

		public BoardCompositeMapConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadEnumerable(
				i => Stream.ReadEnumerable(
					j => new BoardConfiguration(Stream.ReadString(), Stream.ReadBoolean()))))
		{ }

		public Map GenerateMap(IdGenerator IdGenerator)
		{
			List<List<Tuple<Map, bool>>> boards = Boards.Select(i => i.Select(j => j.LoadMap()).ToList()).ToList();

			int width = boards.Max(i => i.Sum(j => j.Item1.Width) - i.Count + 1);
			int height = boards.Sum(i => i.Max(j => j.Item1.Height)) - Boards.Count + 1;

			Map map = new Map(width, height, GameData.TileRuleSet, IdGenerator);

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
			Stream.Write(Boards, i => Stream.Write(i));
		}
	}
}
