using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileInRegion : Matcher<Tile>
	{
		enum Attribute { REGION_NAME }

		public readonly string NormalizedRegionName;

		public override bool IsTransient { get; } = false;

		public TileInRegion(string NormalizedRegionName)
		{
			this.NormalizedRegionName = NormalizedRegionName;
		}

		public TileInRegion(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			NormalizedRegionName = (string)attributes[(int)Attribute.REGION_NAME];
		}

		public TileInRegion(SerializationInputStream Stream)
			: this(Stream.ReadString()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(NormalizedRegionName);
		}

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Object.Map.Regions.First(
				i => NormalizedRegionName == i.Name.Replace(' ', '-').ToLower()).Contains(Object);
		}
	}
}
