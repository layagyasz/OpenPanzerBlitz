using System;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileInRegion : Matcher<Tile>
	{
		enum Attribute { NAME }

		public readonly string NormalizedRegionName;

		public TileInRegion(string NormalizedRegionName)
		{
			this.NormalizedRegionName = NormalizedRegionName;
		}

		public TileInRegion(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			NormalizedRegionName = (string)attributes[(int)Attribute.NAME];
		}

		public TileInRegion(SerializationInputStream Stream)
			: this(Stream.ReadString()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(NormalizedRegionName);
		}

		public bool Matches(Tile Tile)
		{
			return Tile.Map.Regions.First(
				i => NormalizedRegionName == i.Name.Replace(' ', '-').ToLower()).Contains(Tile);
		}
	}
}
