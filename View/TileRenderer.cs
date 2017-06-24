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
		public static Vertex[] Render(Tile Tile)
		{
			List<Vertex> vertices = new List<Vertex>();

			Color baseColor = BaseColor(Tile.TileBase);
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

			// Forest.
			if (Tile.HasEdge(Edge.FOREST))
				RenderTile(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == Edge.FOREST,
					vertices, OverlayColor(Edge.FOREST));
			// Town.
			if (Tile.HasEdge(Edge.TOWN))
				RenderTile(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == Edge.TOWN,
					vertices, OverlayColor(Edge.TOWN));

			// Ridges.
			if (Tile.TileBase != TileBase.SLOPE && Tile.HasEdge(Edge.SLOPE))
				RenderTile(
					Tile,
					(i, j) => j != null && i.GetEdge(j) == Edge.SLOPE,
					vertices, OverlayColor(Edge.SLOPE));

			// IsHilltop.
			bool isHilltop = Tile.NeighborTiles.Any(i => i != null && i.Elevation < Tile.Elevation);

			for (int i = 0; i < Tile.Bounds.Length; ++i)
			{
				// Border.
				Color edgeColor = EdgeColor(Tile.GetEdge(i), isHilltop);
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
			if (points.Count == 0)
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
			else if (points.Count == 2)
			{
				Vector2f point3 = Inlay(points[0], points[1], Tile.Center, .25f);
				Vertices.Add(new Vertex(points[0], Color));
				Vertices.Add(new Vertex(points[1], Color));
				Vertices.Add(new Vertex(point3, Color));
			}
			else
			{
				for (int i = 2; i < points.Count; ++i)
				{
					Vertices.Add(new Vertex(points[i], Color));
					Vertices.Add(new Vertex(points[i - 1], Color));
					Vertices.Add(new Vertex(points[0], Color));
				}
			}
		}

		static Vector2f Inlay(Vector2f Point1, Vector2f Point2, Vector2f Towards, float Distance)
		{
			return Distance * Towards + (1 - Distance) * new Segment(Point1, Point2).Project(Towards);
		}

		static Color BaseColor(TileBase TileConfiguration)
		{
			return new Color(205, 194, 149);
		}

		static Color TopColor(TileBase TileConfiguration)
		{
			if (TileConfiguration == TileBase.SLOPE) return new Color(169, 150, 71);
			else if (TileConfiguration == TileBase.SWAMP) return new Color(145, 155, 130);
			else if (TileConfiguration == TileBase.WATER) return new Color(43, 122, 119);
			else return new Color(255, 255, 255, 0);
		}

		static Color EdgeColor(Edge Edge, bool Higher)
		{
			switch (Edge)
			{
				case Edge.FOREST:
					return new Color(94, 111, 56);
				case Edge.TOWN:
					return new Color(115, 112, 103);
				case Edge.SLOPE:
					return new Color(188, 126, 53);
				default:
					return Higher ? new Color(235, 37, 26) : new Color(0, 0, 0, 0);
			}
		}

		static Color OverlayColor(Edge Edge)
		{
			if (Edge == Edge.FOREST) return new Color(125, 150, 72);
			else if (Edge == Edge.TOWN) return new Color(138, 134, 122);
			else if (Edge == Edge.SLOPE) return new Color(169, 150, 71);
			else return new Color(0, 0, 0, 0);
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
