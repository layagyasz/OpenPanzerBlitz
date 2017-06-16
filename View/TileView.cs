using System;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class TileView : Interactive
	{
		public readonly Tile Tile;
		private Vertex[] _Vertices;
		private int _LineOfSightCount;

		public override Vector2f Size
		{
			get
			{
				return Tile.Bounds.Size;
			}
		}
		public bool InLineOfSight
		{
			get
			{
				return _LineOfSightCount > 0;
			}
		}

		public TileView(Tile Tile)
		{
			this.Tile = Tile;
			_Vertices = new Vertex[Tile.Bounds.Length + 1];
			for (int i = 0; i < _Vertices.Length; ++i)
			{
				_Vertices[i] = new Vertex(Tile.Bounds[i % Tile.Bounds.Length].Point, Color.White);
			}
		}

		public void PutInLineOfSight()
		{
			_LineOfSightCount++;
		}

		public void RemoveLineOfSight()
		{
			_LineOfSightCount--;
			if (_LineOfSightCount < 0) throw new Exception("Line-of-sight counter should not be negative.");
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
			Target.Draw(_Vertices, PrimitiveType.LinesStrip, new RenderStates(Transform));
		}
	}
}
