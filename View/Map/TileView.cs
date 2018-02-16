using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class TileView : Interactive
	{
		public readonly Tile Tile;

		readonly Vertex[] _BorderVertices;

		Vertex[] _Vertices;

		public override Vector2f Size
		{
			get
			{
				return Tile.Bounds.Size;
			}
		}

		public TileView(Tile Tile, TileRenderer TileRenderer)
		{
			this.Tile = Tile;
			Tile.Configuration.OnReconfigure += (sender, e) => Render(TileRenderer);
			foreach (Tile t in Tile.NeighborTiles)
			{
				if (t != null) t.Configuration.OnReconfigure += (sender, e) => Render(TileRenderer);
			}

			_BorderVertices = new Vertex[Tile.Bounds.Length + 1];
			for (int i = 0; i < _BorderVertices.Length; ++i)
			{
				_BorderVertices[i] = new Vertex(Tile.Bounds[i % Tile.Bounds.Length].Point, Color.Black);
			}

			Render(TileRenderer);
		}

		void Render(TileRenderer TileRenderer)
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
			var r = new RenderStates(Transform);
			Target.Draw(_Vertices, PrimitiveType.Triangles, r);
			Target.Draw(_BorderVertices, PrimitiveType.LinesStrip, r);
		}
	}
}
