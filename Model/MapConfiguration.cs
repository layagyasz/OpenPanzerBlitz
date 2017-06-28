using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MapConfiguration
	{
		public readonly List<List<Tuple<string, bool>>> Boards;

		public MapConfiguration(ParseBlock Block)
		{
			Boards = Block.BreakToList<List<Tuple<object, object>>>().Select(
				i => i.Select(
					j => new Tuple<string, bool>((string)j.Item1, (bool)j.Item2)).ToList()).ToList();
		}
	}
}
