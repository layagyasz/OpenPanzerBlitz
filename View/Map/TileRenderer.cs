using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Planar;
using Cardamom.Serialization;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class TileRenderer
	{
		enum Attribute
		{
			FONT_FACE,
			FONT_COLOR,
			BASE_COLOR,
			ELEVATION_COLORS,
			TOP_COLORS,
			EDGE_COLORS,
			EDGE_OVERLAY_COLORS,
			PATH_COLORS,
			PATH_BORDER_COLORS,
			PATH_WIDTHS,
			PATH_BORDER_WIDTHS
		};

		public readonly Font FontFace;
		public readonly Color FontColor;
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

		public TileRenderer(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			FontFace = Cardamom.Interface.ClassLibrary.Instance.GetFont((string)attributes[(int)Attribute.FONT_FACE]);
			FontColor = (Color)attributes[(int)Attribute.FONT_COLOR];
			BaseColor = (Color)attributes[(int)Attribute.BASE_COLOR];
			_ElevationColors = (Color[])attributes[(int)Attribute.ELEVATION_COLORS];
			_TopColors = Parse.KeyByEnum<TileBase, Color>(
				(Dictionary<string, Color>)attributes[(int)Attribute.TOP_COLORS]);
			_EdgeColors = Parse.KeyByEnum<TileEdge, Color>(
				(Dictionary<string, Color>)attributes[(int)Attribute.EDGE_COLORS]);
			_OverlayColors = Parse.KeyByEnum<TileEdge, Color>(
				(Dictionary<string, Color>)attributes[(int)Attribute.EDGE_OVERLAY_COLORS]);
			_PathColors = Parse.KeyByEnum<TilePathOverlay, Color>(
				(Dictionary<string, Color>)attributes[(int)Attribute.PATH_COLORS]);
			_PathBorderColors = Parse.KeyByEnum<TilePathOverlay, Color>(
				(Dictionary<string, Color>)attributes[(int)Attribute.PATH_BORDER_COLORS]);
			_PathWidths = Parse.KeyByEnum<TilePathOverlay, float>(
				(Dictionary<string, float>)attributes[(int)Attribute.PATH_WIDTHS]);
			_PathBorderWidths = Parse.KeyByEnum<TilePathOverlay, float>(
				(Dictionary<string, float>)attributes[(int)Attribute.PATH_BORDER_WIDTHS]);
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
			if (Tile.Configuration.TileBase == TileBase.SLOPE)
				RenderTile(
					Tile,
					(i, j) => j != null && (j.Configuration.TileBase == TileBase.SLOPE
											|| j.Configuration.Elevation > Tile.Configuration.Elevation),
					vertices,
					TopColor(Tile.Configuration.TileBase));
			// Swamp.
			if (Tile.Configuration.TileBase == TileBase.SWAMP)
				RenderTile(
					Tile,
					(i, j) => j != null && j.Configuration.TileBase == TileBase.SWAMP,
					vertices,
					TopColor(Tile.Configuration.TileBase));
			// Village.
			if (Tile.Configuration.TileBase == TileBase.VILLAGE)
				RenderTile(
					Tile,
					(i, j) => j != null && j.Configuration.TileBase == TileBase.VILLAGE,
					vertices,
					TopColor(Tile.Configuration.TileBase));
			// Fort.
			if (Tile.Configuration.TileBase == TileBase.FORT)
				RenderTile(
					Tile,
					(i, j) => false,
					vertices,
					TopColor(Tile.Configuration.TileBase));
			// Wheatfield
			if (Tile.Configuration.TileBase == TileBase.WHEAT_FIELD)
				RenderTile(
					Tile,
					(i, j) => false,
					vertices,
					TopColor(Tile.Configuration.TileBase));
			// Copse.
			if (Tile.Configuration.TileBase == TileBase.COPSE)
				RenderTile(
					Tile,
					(i, j) => false,
					vertices,
					TopColor(Tile.Configuration.TileBase));

			// Forest.
			if (Tile.Configuration.HasEdge(TileEdge.FOREST))
				RenderEdges(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == TileEdge.FOREST,
					vertices,
					OverlayColor(TileEdge.FOREST),
					true);
			// Town.
			if (Tile.Configuration.HasEdge(TileEdge.TOWN))
				RenderEdges(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == TileEdge.TOWN,
					vertices,
					OverlayColor(TileEdge.TOWN),
					false);

			// Ridges.
			if (Tile.Configuration.TileBase != TileBase.SLOPE && Tile.Configuration.HasEdge(TileEdge.SLOPE))
				RenderEdges(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == TileEdge.SLOPE,
					vertices,
					OverlayColor(TileEdge.SLOPE),
					false);

			// Stream Gully.
			if (Tile.Configuration.PathOverlays.Contains(TilePathOverlay.STREAM))
				RenderPath(
					Tile,
					(i, j) => i.Configuration.GetPathOverlay(j) == TilePathOverlay.STREAM
						|| i.Configuration.GetPathOverlay(j) == TilePathOverlay.STREAM_FORD,
					vertices,
					PathBorderColor(TilePathOverlay.STREAM),
					PathBorderWidth(TilePathOverlay.STREAM));

			// Stream.
			if (Tile.Configuration.PathOverlays.Contains(TilePathOverlay.STREAM)
				|| Tile.Configuration.PathOverlays.Contains(TilePathOverlay.STREAM_FORD))
				RenderPath(
					Tile,
					(i, j) => i.Configuration.GetPathOverlay(j) == TilePathOverlay.STREAM
						|| i.Configuration.GetPathOverlay(j) == TilePathOverlay.STREAM_FORD,
					vertices,
					PathColor(TilePathOverlay.STREAM),
					PathWidth(TilePathOverlay.STREAM));

			// Water.
			if (Tile.Configuration.HasEdge(TileEdge.WATER))
				RenderEdges(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == TileEdge.WATER,
					vertices,
					OverlayColor(TileEdge.WATER),
					false);

			// Dirt Road.
			if (Tile.Configuration.PathOverlays.Contains(TilePathOverlay.DIRT_ROAD))
			{
				RenderPath(
					Tile,
					(i, j) => i.Configuration.GetPathOverlay(j) == TilePathOverlay.DIRT_ROAD,
					vertices,
					PathBorderColor(TilePathOverlay.DIRT_ROAD),
					PathBorderWidth(TilePathOverlay.DIRT_ROAD));
				RenderPath(
					Tile,
					(i, j) => i.Configuration.GetPathOverlay(j) == TilePathOverlay.DIRT_ROAD,
					vertices,
					PathColor(TilePathOverlay.DIRT_ROAD),
					PathWidth(TilePathOverlay.DIRT_ROAD));
			}

			// Road.
			if (Tile.Configuration.PathOverlays.Contains(TilePathOverlay.ROAD))
			{
				RenderPath(
					Tile,
					(i, j) => i.Configuration.GetPathOverlay(j) == TilePathOverlay.ROAD,
					vertices,
					PathBorderColor(TilePathOverlay.ROAD),
					PathBorderWidth(TilePathOverlay.ROAD));
				RenderPath(
					Tile,
					(i, j) => i.Configuration.GetPathOverlay(j) == TilePathOverlay.ROAD,
					vertices,
					PathColor(TilePathOverlay.ROAD),
					PathWidth(TilePathOverlay.ROAD));
			}

			// Railroad.
			if (Tile.Configuration.PathOverlays.Contains(TilePathOverlay.RAIL_ROAD))
			{
				RenderPath(
					Tile,
					(i, j) => i.Configuration.GetPathOverlay(j) == TilePathOverlay.RAIL_ROAD,
					vertices,
					PathBorderColor(TilePathOverlay.RAIL_ROAD),
					PathBorderWidth(TilePathOverlay.RAIL_ROAD));
				RenderPath(
					Tile,
					(i, j) => i.Configuration.GetPathOverlay(j) == TilePathOverlay.RAIL_ROAD,
					vertices,
					PathColor(TilePathOverlay.RAIL_ROAD),
					PathWidth(TilePathOverlay.RAIL_ROAD));
			}

			// IsHilltop.
			bool isHilltop = Tile.NeighborTiles.Any(i => i != null
													&& i.Configuration.Elevation < Tile.Configuration.Elevation);

			for (int i = 0; i < Tile.Bounds.Length; ++i)
			{
				// Border.
				Color edgeColor = EdgeColor(Tile.Configuration.GetEdge(i), isHilltop, Tile.Configuration.Elevation);
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

		static void RenderEdges(
			Tile Tile, Func<Tile, Tile, bool> Matched, List<Vertex> Vertices, Color Color, bool Segmented)
		{
			List<List<int>> indices = new List<List<int>>();
			for (int i = 0; i < Tile.NeighborTiles.Length; ++i)
			{
				if (Tile.NeighborTiles[i] != null && Matched(Tile, Tile.NeighborTiles[i]))
				{
					if (indices.Count > 0 && (indices.Last().Last() == i - 1 || !Segmented)) indices.Last().Add(i);
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
			return _TopColors[(int)TileBase];
		}

		Color EdgeColor(TileEdge Edge, bool Higher, int Elevation)
		{
			Color c = _EdgeColors[(int)Edge];
			if (c.A > 0) return c;
			else return Higher ? _ElevationColors[Math.Min(_ElevationColors.Length - 1, Math.Max(0, Elevation))] : c;
		}

		Color OverlayColor(TileEdge Edge)
		{
			return _OverlayColors[(int)Edge];
		}

		Color PathColor(TilePathOverlay PathOverlay)
		{
			return _PathColors[(int)PathOverlay];
		}

		Color PathBorderColor(TilePathOverlay PathOverlay)
		{
			return _PathBorderColors[(int)PathOverlay];
		}

		float PathWidth(TilePathOverlay PathOverlay)
		{
			return _PathWidths[(int)PathOverlay];

		}

		float PathBorderWidth(TilePathOverlay PathOverlay)
		{
			return _PathBorderWidths[(int)PathOverlay];
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
