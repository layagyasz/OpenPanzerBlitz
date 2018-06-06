using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Utilities;

using Cence;

using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitConfigurationView : GuiItem
	{
		public readonly float Scale;

		readonly Texture _Texture;
		readonly Rectangle _Bounds;
		readonly Vertex[] _ImageVertices;
		readonly Vertex[] _Vertices;

		public bool Flipped;

		public override Vector2f Size
		{
			get
			{
				return _Bounds.Size;
			}
		}

		public UnitConfigurationView(
			UnitConfiguration UnitConfiguration, Faction Faction, UnitConfigurationRenderer Renderer, float Scale)
		{
			this.Scale = Scale;
			Color[] colors = UnitConfiguration.UnitClass == UnitClass.BLOCK
											  || UnitConfiguration.UnitClass == UnitClass.MINEFIELD
											  ? new Color[] { Color.White } : Faction.Colors;
			if (!UnitConfiguration.IsVehicle)
			{
				colors = colors.ToArray();
				for (int i = 0; i < colors.Length; ++i)
				{
					var f = new FloatingColor(colors[i]);
					f = f.MakeHSL();
					f.B = (float)Math.Min(1, f.B + .1);
					colors[i] = f.MakeRGB().ConvertToColor();
				}
			}

			_Vertices = new Vertex[colors.Length * 4];
			float barHeight = 1f / colors.Length;

			var renderInfo = Renderer.GetRenderInfo(UnitConfiguration);
			_Texture = renderInfo.Item1;
			for (int i = 0; i < colors.Length; ++i)
			{
				_Vertices[i * 4] = new Vertex(new Vector2f(-.5f, i * barHeight - .5f), colors[i]);
				_Vertices[i * 4 + 1] = new Vertex(new Vector2f(.5f, i * barHeight - .5f), colors[i]);
				_Vertices[i * 4 + 2] = new Vertex(new Vector2f(.5f, (i + 1) * barHeight - .5f), colors[i]);
				_Vertices[i * 4 + 3] = new Vertex(new Vector2f(-.5f, (i + 1) * barHeight - .5f), colors[i]);
			}
			_ImageVertices = new Vertex[4];
			Color c = Renderer.RenderDetails[UnitConfiguration.UniqueKey].OverrideColor;
			if (c.R == 0 && c.G == 0 && c.B == 0)
				c = colors.ArgMax(i => new FloatingColor(i).Luminosity());

			var tl = new Vector2f(-.5f, -.5f);
			var tr = new Vector2f(.5f, -.5f);
			var br = new Vector2f(.5f, .5f);
			var bl = new Vector2f(-.5f, .5f);
			_ImageVertices[0] = new Vertex(tl, c, renderInfo.Item2[0]);
			_ImageVertices[1] = new Vertex(tr, c, renderInfo.Item2[1]);
			_ImageVertices[2] = new Vertex(br, c, renderInfo.Item2[2]);
			_ImageVertices[3] = new Vertex(bl, c, renderInfo.Item2[3]);

			_Bounds = new Rectangle(new Vector2f(-.5f, -.5f) * Scale, new Vector2f(1, 1) * Scale);
		}

		public void SetAlpha(byte Alpha)
		{
			for (int i = 0; i < _Vertices.Length; ++i) _Vertices[i].Color.A = Alpha;
			for (int i = 0; i < _ImageVertices.Length; ++i) _ImageVertices[i].Color.A = Alpha;
		}

		public bool IsCollision(Vector2f Point)
		{
			return _Bounds.ContainsPoint(Point);
		}

		public override void Update(
			MouseController MouseController,
			KeyController KeyController,
			int DeltaT,
			Transform Transform)
		{ }

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			Transform.Scale(Scale, Scale);
			var r = new RenderStates();
			r.Transform = Transform;

			Target.Draw(_Vertices, PrimitiveType.Quads, r);

			if (!Flipped)
			{
				r.Texture = _Texture;
				Target.Draw(_ImageVertices, PrimitiveType.Quads, r);
			}
		}
	}
}
