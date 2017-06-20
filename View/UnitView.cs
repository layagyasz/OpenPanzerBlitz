using System;

using Cardamom.Interface;
using Cardamom.Planar;

using Cence;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitView : Interactive
	{
		Texture _Texture;
		Vertex[] _ImageVertices;
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

		public UnitView(Unit Unit, UnitConfigurationRenderer Renderer, float Size)
		{
			this.Unit = Unit;
			Color[] colors = Unit.Army.ArmyConfiguration.Faction.Colors;
			_Vertices = new Vertex[colors.Length * 4];
			float barHeight = 1f / colors.Length;

			var renderInfo = Renderer.GetRenderInfo(Unit.UnitConfiguration);
			_Texture = renderInfo.Item1;
			_Texture.CopyToImage().SaveToFile("out0.png");
			for (int i = 0; i < colors.Length; ++i)
			{
				Color color = colors[i];
				if (!Unit.UnitConfiguration.IsArmored)
				{
					FloatingColor f = new FloatingColor(color);
					f = f.MakeHSL();
					f.B = (float)Math.Max(0, f.B + .1);
					color = f.MakeRGB().ConvertToColor();
				}
				_Vertices[i * 4] = new Vertex(new Vector2f(-.5f, i * barHeight - .5f) * Size, color);
				_Vertices[i * 4 + 1] = new Vertex(new Vector2f(.5f, i * barHeight - .5f) * Size, color);
				_Vertices[i * 4 + 2] = new Vertex(new Vector2f(.5f, (i + 1) * barHeight - .5f) * Size, color);
				_Vertices[i * 4 + 3] = new Vertex(new Vector2f(-.5f, (i + 1) * barHeight - .5f) * Size, color);
			}
			_ImageVertices = new Vertex[4];
			_ImageVertices[0] = new Vertex(
				new Vector2f(-.5f, -.5f) * Size, Color.White, renderInfo.Item2[0]);
			_ImageVertices[1] = new Vertex(
				new Vector2f(.5f, -.5f) * Size, Color.White, renderInfo.Item2[1]);
			_ImageVertices[2] = new Vertex(
				new Vector2f(.5f, .5f) * Size, Color.White, renderInfo.Item2[2]);
			_ImageVertices[3] = new Vertex(
				new Vector2f(-.5f, .5f) * Size, Color.White, renderInfo.Item2[3]);

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
			RenderStates r = new RenderStates();
			r.Transform = Transform;

			Target.Draw(_Vertices, PrimitiveType.Quads, r);

			r.Texture = _Texture;
			Target.Draw(_ImageVertices, PrimitiveType.Quads, r);
		}
	}
}
