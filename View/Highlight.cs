using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class Highlight
	{
		public readonly List<Tuple<Tile, Color>> Highlights;

		public Highlight()
		{
			Highlights = new List<Tuple<Tile, Color>>();
		}

		public Highlight(IEnumerable<Tuple<Tile, Color>> Highlights)
		{
			this.Highlights = Highlights.ToList();
		}
	}
}
