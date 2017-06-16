using System;

using Cardamom.Interface;
using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitView : Interactive
	{
		Vertex[] _Vertices;
		Rectangle _Bounds;

		public readonly Unit Unit;

		public override Vector2f Size
		{
			get
			{
				return _Bounds.Size;
			}
		}

		public UnitView(Unit Unit, float Size)
		{
			this.Unit = Unit;
			Color[] colors = Unit.Army.ArmyConfiguration.Faction.Colors;
			_Vertices = new Vertex[colors.Length * 4];
			float barHeight = 1f / colors.Length;
			for (int i = 0; i < colors.Length; ++i)
			{
				_Vertices[i * 4] = new Vertex(new Vector2f(-.5f, i * barHeight - .5f) * Size, colors[i]);
				_Vertices[i * 4 + 1] = new Vertex(new Vector2f(.5f, i * barHeight - .5f) * Size, colors[i]);
				_Vertices[i * 4 + 2] = new Vertex(new Vector2f(.5f, (i + 1) * barHeight - .5f) * Size, colors[i]);
				_Vertices[i * 4 + 3] = new Vertex(new Vector2f(-.5f, (i + 1) * barHeight - .5f) * Size, colors[i]);
			}
			_Bounds = new Rectangle(new Vector2f(-.5f, -.5f) * Size, new Vector2f(1, 1) * Size);
		}

		public override bool IsCollision(Vector2f Point)
		{
			return _Bounds.ContainsPoint(Point);
		}

		public override void Update(
			MouseController MouseController,
			KeyController KeyController,
			int DeltaT,
			Transform Transform)
		{
			Transform.Translate(Position);
			base.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			Target.Draw(_Vertices, PrimitiveType.Quads, new RenderStates(Transform));
		}
	}
}
