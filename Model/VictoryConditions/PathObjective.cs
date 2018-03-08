using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class PathObjective : Objective
	{
		enum Attribute { SOURCE, SINK, PATH }

		public readonly Matcher<Tile> Source;
		public readonly Matcher<Tile> Sink;
		public readonly Matcher<Tile> Path;

		public PathObjective(Matcher<Tile> Source, Matcher<Tile> Sink, Matcher<Tile> Path)
		{
			this.Source = Source;
			this.Sink = Sink;
			this.Path = Path;
		}

		public PathObjective(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Source = (Matcher<Tile>)attributes[(int)Attribute.SOURCE];
			Sink = (Matcher<Tile>)attributes[(int)Attribute.SINK];
			Path = (Matcher<Tile>)attributes[(int)Attribute.PATH];
		}

		public PathObjective(SerializationInputStream Stream)
			: this(
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream),
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream),
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream))
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Source, Stream);
			MatcherSerializer.Instance.Serialize(Sink, Stream);
			MatcherSerializer.Instance.Serialize(Path, Stream);
		}

		public override bool CanStopEarly()
		{
			return false;
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			var source = new Tile(Match.Map, new Coordinate(-1, -1), null, new IdGenerator());
			var sink = new Tile(Match.Map, new Coordinate(-1, -1), null, new IdGenerator());
			var sources = new HashSet<Tile>(Match.Map.TilesEnumerable.Where(i => Source.Matches(i)));
			var sinks = new HashSet<Tile>(Match.Map.TilesEnumerable.Where(i => Sink.Matches(i)));

			var path = new Path<Tile>(
				source,
				sink,
				i => true,
				(i, j) => GetDistance(i, j, source, sink),
				(i, j) => 1, i => GetNeighbors(source, sink, sources, sinks, i),
				(i, j) => i == j);
			return path.Distance < double.MaxValue ? 1 : 0;
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Map.TilesEnumerable.Where(i => Path.Matches(i));
		}

		double GetDistance(Tile From, Tile To, Tile Source, Tile Sink)
		{
			if (From == Source || To == Sink) return 1;
			if (From == Sink || To == Source) return double.MaxValue;
			return Path.Matches(To) ? 1 : double.MaxValue;
		}

		IEnumerable<Tile> GetNeighbors(Tile Source, Tile Sink, HashSet<Tile> Sources, HashSet<Tile> Sinks, Tile Tile)
		{
			if (Tile == Source)
			{
				foreach (Tile t in Sources) yield return t;
			}
			if (Sinks.Contains(Tile)) yield return Sink;
			foreach (Tile t in Tile.Neighbors()) yield return t;
		}
	}
}
