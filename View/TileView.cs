using System;

using Cardamom.Interface;
using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class TileView : Interactive
	{
		public readonly Tile Tile;

		Vertex[] _BorderVertices;
		Vertex[] _Vertices;

		public override Vector2f Size
		{
			get
			{
				return Tile.Bounds.Size;
			}
		}

		public TileView(Tile Tile)
		{
			this.Tile = Tile;
			Tile.OnReconfigure += (sender, e) => Render();

			_BorderVertices = new Vertex[Tile.Bounds.Length + 1];
			for (int i = 0; i < _BorderVertices.Length; ++i)
			{
				_BorderVertices[i] = new Vertex(Tile.Bounds[i % Tile.Bounds.Length].Point, Color.Black);
			}
			_Vertices = new Vertex[Tile.Bounds.Length * 9];
			for (int i = 0; i < Tile.Bounds.Length; ++i)
			{
				// Body.
				_Vertices[i * 9] = new Vertex(Tile.Bounds[i].Point);
				_Vertices[i * 9 + 1] = new Vertex(Tile.Bounds[i].End);
				_Vertices[i * 9 + 2] = new Vertex(Tile.Center);

				// Border.
				Segment left = Tile.Bounds[Mod(i - 1, Tile.Bounds.Length)];
				Segment right = Tile.Bounds[Mod(i + 1, Tile.Bounds.Length)];
				Vector2f internalLeft =
					Tile.Bounds[i].Point + .05f * Normalize(
						(Tile.Bounds[i].End - Tile.Bounds[i].Point) + (left.Point - left.End));
				Vector2f internalRight =
					Tile.Bounds[i].End + .05f * Normalize(
						(right.End - right.Point) + (Tile.Bounds[i].Point - Tile.Bounds[i].End));

				_Vertices[i * 9 + 3] = new Vertex(Tile.Bounds[i].Point);
				_Vertices[i * 9 + 4] = new Vertex(Tile.Bounds[i].End);
				_Vertices[i * 9 + 5] = new Vertex(internalLeft);
				_Vertices[i * 9 + 6] = new Vertex(internalLeft);
				_Vertices[i * 9 + 7] = new Vertex(Tile.Bounds[i].End);
				_Vertices[i * 9 + 8] = new Vertex(internalRight);
			}
			Render();
		}

		int Mod(int x, int m)
		{
			int r = x % m;
			return r < 0 ? r + m : r;
		}

		Vector2f Normalize(Vector2f Vector)
		{
			return Vector / (float)Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y);
		}

		void Render()
		{
			Color bodyColor;
			if (Tile.TileConfiguration == TileConfiguration.CLEAR) bodyColor = Color.White;
			else if (Tile.TileConfiguration == TileConfiguration.SLOPE) bodyColor = Color.Red;
			else if (Tile.TileConfiguration == TileConfiguration.SWAMP) bodyColor = Color.Green;
			else if (Tile.TileConfiguration == TileConfiguration.WATER) bodyColor = Color.Blue;
			else bodyColor = Color.Black;

			for (int i = 0; i < Tile.Bounds.Length; ++i)
			{
				Color borderColor;
				Edge e = Tile.GetEdge(i);
				switch (e)
				{
					case Edge.FOREST:
						borderColor = Color.Green;
						break;
					case Edge.TOWN:
						borderColor = Color.Yellow;
						break;
					case Edge.SLOPE:
						borderColor = Color.Magenta;
						break;
					default:
						borderColor = new Color(0, 0, 0, 0);
						break;
				}

				for (int j = 0; j < 3; ++j)
				{
					_Vertices[i * 9 + j].Color = bodyColor;
				}
				for (int j = 3; j < 9; ++j)
				{
					_Vertices[i * 9 + j].Color = borderColor;
				}
			}
		}

		public override bool IsCollision(Vector2f Point)
		{
			return Tile.Bounds.ContainsPoint(Point);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			RenderStates r = new RenderStates(Transform);
			Target.Draw(_Vertices, PrimitiveType.Triangles, r);
			Target.Draw(_BorderVertices, PrimitiveType.LinesStrip, r);
		}
	}
}
