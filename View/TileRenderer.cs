using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class TileRenderer
	{
		public static readonly TileRenderer SUMMER_STEPPE = new TileRenderer(
				new Color(205, 194, 149),
				new Color[] { Color.Black, new Color(235, 37, 26) },
				new Color[]
				{
					Color.White,
					new Color(145, 155, 130),
					new Color(43, 122, 119),
					new Color(169, 150, 71),
					Color.White
				},
				new Color[]
				{
					new Color(0, 0, 0, 0),
					new Color(115, 112, 103),
					new Color(94, 111, 56),
					new Color(188, 126, 53)
				},
				new Color[]
				{
					new Color(0, 0, 0, 0),
					new Color(138, 134, 122),
					new Color(125, 150, 72),
					new Color(169, 150, 71)
				},
				new Color[] { new Color(0, 0, 0, 0), new Color(220, 220, 220), new Color(43, 122, 119) },
				new Color[] { new Color(0, 0, 0, 0), Color.Black, new Color(160, 160, 160) },
				new float[] { 0, .15f, .15f },
				new float[] { 0, .2f, .3f }
		);

		public static readonly TileRenderer WINTER_STEPPE = new TileRenderer(
				new Color(217, 211, 195),
				new Color[] { Color.Black, new Color(235, 37, 26) },
				new Color[]
				{
					Color.White,
					new Color(145, 155, 130),
					new Color(43, 122, 119),
					new Color(160, 160, 160),
					Color.White
				},
				new Color[]
				{
					new Color(0, 0, 0, 0),
					new Color(115, 112, 103),
					new Color(94, 111, 56),
					new Color(188, 126, 53)
				},
				new Color[]
				{
					new Color(0, 0, 0, 0),
					new Color(138, 134, 122),
					new Color(125, 150, 72),
					new Color(160, 160, 160)
				},
				new Color[] { new Color(0, 0, 0, 0), new Color(195, 159, 109), new Color(140, 200, 200) },
				new Color[] { new Color(0, 0, 0, 0), Color.Black, new Color(160, 160, 160) },
				new float[] { 0, .15f, .15f },
				new float[] { 0, .2f, .3f }
		);


		public readonly Color BaseColor;

		Color[] _ElevationColors;
		Color[] _TopColors;
		Color[] _EdgeColors;
		Color[] _OverlayColors;
		Color[] _PathColors;
		Color[] _PathBorderColors;
		float[] _PathWidths;
		float[] _PathBorderWidths;

		public TileRenderer(
			Color BaseColor,
			Color[] ElevationColors,
			Color[] TopColors,
			Color[] EdgeColors,
			Color[] OverlayColors,
			Color[] PathColors,
			Color[] PathBorderColors,
			float[] PathWidths,
			float[] PathBorderWidths)
		{
			this.BaseColor = BaseColor;
			_ElevationColors = ElevationColors;
			_TopColors = TopColors;
			_EdgeColors = EdgeColors;
			_OverlayColors = OverlayColors;
			_PathColors = PathColors;
			_PathBorderColors = PathBorderColors;
			_PathWidths = PathWidths;
			_PathBorderWidths = PathBorderWidths;
		}

		public Vertex[] Render(Tile Tile)
		{
			List<Vertex> vertices = new List<Vertex>();

			Color baseColor = BaseColor;
			for (int i = 0; i < Tile.Bounds.Length; ++i)
			{
				// Body.
				vertices.Add(new Vertex(Tile.Bounds[i].Point, baseColor));
				vertices.Add(new Vertex(Tile.Bounds[i].End, baseColor));
				vertices.Add(new Vertex(Tile.Center, baseColor));
			}

			// Slopes.
			if (Tile.TileBase == TileBase.SLOPE)
				RenderTile(
					Tile,
					(i, j) => j != null && (j.TileBase == TileBase.SLOPE || j.Elevation > Tile.Elevation),
					vertices,
					TopColor(Tile.TileBase));
			// Swamp.
			if (Tile.TileBase == TileBase.SWAMP)
				RenderTile(
					Tile,
					(i, j) => j != null && j.TileBase == TileBase.SWAMP,
					vertices,
					TopColor(Tile.TileBase));

			// Forest.
			if (Tile.HasEdge(Edge.FOREST))
				RenderEdges(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == Edge.FOREST,
					vertices, OverlayColor(Edge.FOREST));
			// Town.
			if (Tile.HasEdge(Edge.TOWN))
				RenderEdges(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == Edge.TOWN,
					vertices, OverlayColor(Edge.TOWN));

			// Ridges.
			if (Tile.TileBase != TileBase.SLOPE && Tile.HasEdge(Edge.SLOPE))
				RenderEdges(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == Edge.SLOPE,
					vertices, OverlayColor(Edge.SLOPE));

			// Stream Gully.
			if (Tile.PathOverlays.Contains(TilePathOverlay.STREAM) && Tile.TileBase != TileBase.STREAM_FORD)
				RenderPath(
					Tile,
					(i, j) => i.GetPathOverlay(j) == TilePathOverlay.STREAM,
					vertices,
					PathBorderColor(TilePathOverlay.STREAM),
					PathBorderWidth(TilePathOverlay.STREAM));

			// Stream.
			if (Tile.PathOverlays.Contains(TilePathOverlay.STREAM))
				RenderPath(
					Tile,
					(i, j) => i.GetPathOverlay(j) == TilePathOverlay.STREAM,
					vertices,
					PathColor(TilePathOverlay.STREAM),
					PathWidth(TilePathOverlay.STREAM));

			// Shore.
			if (Tile.NeighborTiles.Count(i => i != null && i.TileBase == TileBase.WATER) > 0)
				RenderTile(
					Tile,
					(i, j) => j != null && j.TileBase == TileBase.WATER,
					vertices,
					TopColor(TileBase.WATER));
			// Water.
			if (Tile.TileBase == TileBase.WATER)
				RenderTile(Tile, (i, j) => true, vertices, TopColor(Tile.TileBase));

			// Road.
			if (Tile.PathOverlays.Contains(TilePathOverlay.ROAD))
			{
				RenderPath(
					Tile,
					(i, j) => i.GetPathOverlay(j) == TilePathOverlay.ROAD,
					vertices,
					PathBorderColor(TilePathOverlay.ROAD),
					PathBorderWidth(TilePathOverlay.ROAD));
				RenderPath(
					Tile,
					(i, j) => i.GetPathOverlay(j) == TilePathOverlay.ROAD,
					vertices,
					PathColor(TilePathOverlay.ROAD),
					PathWidth(TilePathOverlay.ROAD));
			}

			// IsHilltop.
			bool isHilltop = Tile.NeighborTiles.Any(i => i != null && i.Elevation < Tile.Elevation);

			for (int i = 0; i < Tile.Bounds.Length; ++i)
			{
				// Border.
				Color edgeColor = EdgeColor(Tile.GetEdge(i), isHilltop, Tile.Elevation);
				Segment left = Tile.Bounds[Mod(i - 1, Tile.Bounds.Length)];
				Segment right = Tile.Bounds[Mod(i + 1, Tile.Bounds.Length)];
				Vector2f internalLeft =
					Tile.Bounds[i].Point + .05f * Normalize(
						(Tile.Bounds[i].End - Tile.Bounds[i].Point) + (left.Point - left.End));
				Vector2f internalRight =
					Tile.Bounds[i].End + .05f * Normalize(
						(right.End - right.Point) + (Tile.Bounds[i].Point - Tile.Bounds[i].End));

				vertices.Add(new Vertex(Tile.Bounds[i].Point, edgeColor));
				vertices.Add(new Vertex(Tile.Bounds[i].End, edgeColor));
				vertices.Add(new Vertex(internalLeft, edgeColor));
				vertices.Add(new Vertex(internalLeft, edgeColor));
				vertices.Add(new Vertex(Tile.Bounds[i].End, edgeColor));
				vertices.Add(new Vertex(internalRight, edgeColor));
			}

			return vertices.ToArray();
		}

		static void RenderTile(Tile Tile, Func<Tile, Tile, bool> Matched, List<Vertex> Vertices, Color Color)
		{
			List<Vector2f> points = new List<Vector2f>();
			for (int i = 0; i < Tile.Bounds.Length; ++i)
			{
				if (Matched(Tile, Tile.NeighborTiles[i]))
				{
					points.Add(Tile.Bounds[i].Point);
					points.Add(Tile.Bounds[i].End);
				}
			}
			RenderPoints(Tile, points, Vertices, Color);
		}

		static void RenderEdges(Tile Tile, Func<Tile, Tile, bool> Matched, List<Vertex> Vertices, Color Color)
		{
			List<List<int>> indices = new List<List<int>>();
			for (int i = 0; i < Tile.NeighborTiles.Length; ++i)
			{
				if (Tile.NeighborTiles[i] != null && Matched(Tile, Tile.NeighborTiles[i]))
				{
					if (indices.Count > 0 && indices.Last().Last() == i - 1) indices.Last().Add(i);
					else indices.Add(new List<int>() { i });
				}
			}
			if (indices.Count > 1 && indices.First().First() == (indices.Last().Last() + 1) % Tile.NeighborTiles.Length)
			{
				indices.First().AddRange(indices.Last());
				indices.RemoveAt(indices.Count - 1);
			}
			foreach (List<int> p in indices)
			{
				List<Vector2f> points = new List<Vector2f>();
				foreach (int i in p)
				{
					points.Add(Tile.Bounds[i].Point);
					points.Add(Tile.Bounds[i].End);
				}
				RenderPoints(Tile, points, Vertices, Color);
			}
		}

		static void RenderPoints(Tile Tile, List<Vector2f> Points, List<Vertex> Vertices, Color Color)
		{
			if (Points.Count == 0)
			{
				for (int i = 0; i < Tile.Bounds.Length; ++i)
				{
					Vertices.Add(new Vertex(Inlay(Tile.Bounds[i].Point, Tile.Bounds[i].End, Tile.Center, .5f), Color));
					Vertices.Add(new Vertex(
						Inlay(
							Tile.Bounds[(i + 1) % Tile.Bounds.Length].Point,
							Tile.Bounds[(i + 1) % Tile.Bounds.Length].End,
							Tile.Center, .5f),
						Color));
					Vertices.Add(new Vertex(Tile.Center, Color));
				}
			}
			else if (Points.Count == 2)
			{
				Vector2f point3 = Inlay(Points[0], Points[1], Tile.Center, .25f);
				Vertices.Add(new Vertex(Points[0], Color));
				Vertices.Add(new Vertex(Points[1], Color));
				Vertices.Add(new Vertex(point3, Color));
			}
			else
			{
				for (int i = 2; i < Points.Count; ++i)
				{
					Vertices.Add(new Vertex(Points[i], Color));
					Vertices.Add(new Vertex(Points[i - 1], Color));
					Vertices.Add(new Vertex(Points[0], Color));
				}
			}
		}

		static Vector2f Inlay(Vector2f Point1, Vector2f Point2, Vector2f Towards, float Distance)
		{
			return Distance * Towards + (1 - Distance) * new Segment(Point1, Point2).Project(Towards);
		}

		static void RenderPath(
			Tile Tile, Func<Tile, int, bool> Matched, List<Vertex> Vertices, Color Color, float Width)
		{
			float width = .5f * (1 - Width);

			List<Segment> segments = new List<Segment>();
			for (int i = 0; i < Tile.NeighborTiles.Length; ++i)
			{
				if (Matched(Tile, i)) segments.Add(Tile.Bounds[i]);
			}

			if (segments.Count == 0 || segments.Count > 2)
			{
				for (int i = 0; i < Tile.Bounds.Length; ++i)
				{
					Vertices.Add(
						new Vertex(Tile.Bounds[i].Point * Width + Tile.Center * (1 - Width), Color));
					Vertices.Add(
						new Vertex(Tile.Bounds[i].End * Width + Tile.Center * (1 - Width), Color));
					Vertices.Add(new Vertex(Tile.Center, Color));
				}
			}
			else if (segments.Count == 1)
			{
				Vector2f p1 = OnSegment(segments[0], width);
				Vector2f p2 = OnSegment(segments[0], 1 - width);
				Vertices.Add(new Vertex(p1, Color));
				Vertices.Add(new Vertex(p2, Color));
				Vertices.Add(new Vertex(Tile.Center, Color));
			}
			else if (segments.Count == 2)
			{
				Vector2f p1 = OnSegment(segments[1], width);
				Vector2f p2 = OnSegment(segments[1], 1 - width);
				Vector2f p3 = OnSegment(segments[0], width);
				Vector2f p4 = OnSegment(segments[0], 1 - width);
				Vertices.Add(new Vertex(p1, Color));
				Vertices.Add(new Vertex(p2, Color));
				Vertices.Add(new Vertex(p3, Color));
				Vertices.Add(new Vertex(p4, Color));
				Vertices.Add(new Vertex(p3, Color));
				Vertices.Add(new Vertex(p1, Color));
			}
			if (segments.Count > 2)
			{
				foreach (Segment s in segments)
				{
					Vector2f p1 = OnSegment(s, width);
					Vector2f p2 = OnSegment(s, 1 - width);
					Vector2f p3 = s.Point * Width + Tile.Center * (1 - Width);
					Vector2f p4 = s.End * Width + Tile.Center * (1 - Width);
					Vertices.Add(new Vertex(p1, Color));
					Vertices.Add(new Vertex(p2, Color));
					Vertices.Add(new Vertex(p3, Color));
					Vertices.Add(new Vertex(p4, Color));
					Vertices.Add(new Vertex(p3, Color));
					Vertices.Add(new Vertex(p2, Color));
				}
			}
		}

		static Vector2f OnSegment(Segment Segment, float Distance)
		{
			return Segment.Point + Distance * Segment.Length * Segment.Direction;
		}

		Color TopColor(TileBase TileBase)
		{
			return _TopColors[Array.IndexOf(TileBase.TILE_BASES, TileBase)];
		}

		Color EdgeColor(Edge Edge, bool Higher, int Elevation)
		{
			Color c = _EdgeColors[Array.IndexOf(Edge.EDGES, Edge)];
			if (c.A > 0) return c;
			else return Higher ? _ElevationColors[Math.Min(_ElevationColors.Length - 1, Math.Max(0, Elevation))] : c;
		}

		Color OverlayColor(Edge Edge)
		{
			return _OverlayColors[Array.IndexOf(Edge.EDGES, Edge)];
		}

		Color PathColor(TilePathOverlay PathOverlay)
		{
			return _PathColors[Array.IndexOf(TilePathOverlay.PATH_OVERLAYS, PathOverlay)];
		}

		Color PathBorderColor(TilePathOverlay PathOverlay)
		{
			return _PathBorderColors[Array.IndexOf(TilePathOverlay.PATH_OVERLAYS, PathOverlay)];
		}

		float PathWidth(TilePathOverlay PathOverlay)
		{
			return _PathWidths[Array.IndexOf(TilePathOverlay.PATH_OVERLAYS, PathOverlay)];

		}

		float PathBorderWidth(TilePathOverlay PathOverlay)
		{
			return _PathBorderWidths[Array.IndexOf(TilePathOverlay.PATH_OVERLAYS, PathOverlay)];
		}

		static int Mod(int x, int m)
		{
			int r = x % m;
			return r < 0 ? r + m : r;
		}

		static Vector2f Normalize(Vector2f Vector)
		{
			return Vector / (float)Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y);
		}
	}
}
