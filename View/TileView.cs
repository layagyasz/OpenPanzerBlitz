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
			foreach (Tile t in Tile.NeighborTiles)
			{
				if (t != null) t.OnReconfigure += (sender, e) => Render();
			}

			_BorderVertices = new Vertex[Tile.Bounds.Length + 1];
			for (int i = 0; i < _BorderVertices.Length; ++i)
			{
				_BorderVertices[i] = new Vertex(Tile.Bounds[i % Tile.Bounds.Length].Point, Color.Black);
			}

			Render();
		}

		void Render()
		{
			_Vertices = TileRenderer.Render(Tile);
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
