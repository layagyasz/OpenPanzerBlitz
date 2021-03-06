﻿using System;
using System.Collections.Generic;
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

		public MapConfiguration MakeStatic(Random Random)
		{
			return this;
		}

		public Map GenerateMap(Random Random, Environment Environment, IdGenerator IdGenerator)
		{
			var boards = Boards.Select(i => i.Select(j => j.LoadMap()).ToList()).ToList();

			var width = boards.Max(i => i.Sum(j => j.Item1.Width) - i.Count + 1);
			var height = boards.Sum(i => i.Max(j => j.Item1.Height)) - Boards.Count + 1;

			var map = new Map(width, height, Environment, IdGenerator);

			int rowY = 0;
			foreach (List<Tuple<Map, bool>> mapRow in boards)
			{
				int rowX = 0;
				int nextRowY = 0;
				foreach (Tuple<Map, bool> m in mapRow)
				{
					map.CopyTo(m.Item1, rowX, rowY, m.Item2);
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
