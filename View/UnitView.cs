using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Planar;
using Cardamom.Utilities;

using Cence;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitView : Interactive
	{
		Vertex[] _FiredRect;
		Vertex[] _MovedRect;

		Texture _Texture;
		Vertex[] _ImageVertices;
		Vertex[] _Vertices;
		Rectangle _Bounds;

		MovementDolly _Movement;

		public readonly float Scale;
		public readonly Unit Unit;

		public override Vector2f Size
		{
			get
			{
				return _Bounds.Size;
			}
		}

		public UnitView(Unit Unit, UnitConfigurationRenderer Renderer, float Scale)
		{
			this.Unit = Unit;
			Unit.OnMove += HandleMove;

			Color[] colors = Unit.Configuration.UnitClass == UnitClass.BLOCK
								 || Unit.Configuration.UnitClass == UnitClass.MINEFIELD
								 ? new Color[] { Color.White }
				: Unit.Army.Configuration.Faction.Colors;
			if (!Unit.Configuration.IsVehicle)
			{
				colors = colors.ToArray();
				for (int i = 0; i < colors.Length; ++i)
				{
					FloatingColor f = new FloatingColor(colors[i]);
					f = f.MakeHSL();
					f.B = (float)Math.Min(1, f.B + .1);
					colors[i] = f.MakeRGB().ConvertToColor();
				}
			}

			_Vertices = new Vertex[colors.Length * 4];
			float barHeight = 1f / colors.Length;

			var renderInfo = Renderer.GetRenderInfo(Unit.Configuration);
			_Texture = renderInfo.Item1;
			for (int i = 0; i < colors.Length; ++i)
			{
				_Vertices[i * 4] = new Vertex(new Vector2f(-.5f, i * barHeight - .5f) * Scale, colors[i]);
				_Vertices[i * 4 + 1] = new Vertex(new Vector2f(.5f, i * barHeight - .5f) * Scale, colors[i]);
				_Vertices[i * 4 + 2] = new Vertex(new Vector2f(.5f, (i + 1) * barHeight - .5f) * Scale, colors[i]);
				_Vertices[i * 4 + 3] = new Vertex(new Vector2f(-.5f, (i + 1) * barHeight - .5f) * Scale, colors[i]);
			}
			_ImageVertices = new Vertex[4];
			Color c = Unit.Configuration.OverrideColor;
			if (c.R == 0 && c.G == 0 && c.B == 0)
				c = colors.ArgMax(i => new FloatingColor(i).Luminosity());

			Vector2f tl = new Vector2f(-.5f, -.5f) * Scale;
			Vector2f tr = new Vector2f(.5f, -.5f) * Scale;
			Vector2f br = new Vector2f(.5f, .5f) * Scale;
			Vector2f bl = new Vector2f(-.5f, .5f) * Scale;
			_ImageVertices[0] = new Vertex(tl, c, renderInfo.Item2[0]);
			_ImageVertices[1] = new Vertex(tr, c, renderInfo.Item2[1]);
			_ImageVertices[2] = new Vertex(br, c, renderInfo.Item2[2]);
			_ImageVertices[3] = new Vertex(bl, c, renderInfo.Item2[3]);

			tl = new Vector2f(-.5f, -.15f) * Scale;
			tr = new Vector2f(.5f, -.15f) * Scale;
			br = new Vector2f(.5f, .15f) * Scale;
			bl = new Vector2f(-.5f, .15f) * Scale;
			_FiredRect = new Vertex[]
			{
				new Vertex(tl, Color.Red),
				new Vertex(tr, Color.Red),
				new Vertex(br, Color.Red),
				new Vertex(bl, Color.Red)
			};
			_MovedRect = new Vertex[]
			{
				new Vertex(tl, Color.Blue),
				new Vertex(tr, Color.Blue),
				new Vertex(br, Color.Blue),
				new Vertex(bl, Color.Blue)
			};

			_Bounds = new Rectangle(new Vector2f(-.5f, -.5f) * Scale, new Vector2f(1, 1) * Scale);
		}

		void HandleMove(object Sender, MovementEventArgs E)
		{
			if (E.Path != null) _Movement = new MovementDolly(this, E.Path);
			else Position = E.Tile.Center;
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
			if (_Movement != null)
			{
				Position = _Movement.GetPoint(DeltaT);
				if (_Movement.Done) _Movement = null;
			}
			Transform.Translate(Position);
			base.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			RenderStates r = new RenderStates();
			r.Transform = Transform;

			Target.Draw(_Vertices, PrimitiveType.Quads, r);

			if (Unit.Status != UnitStatus.DISRUPTED)
			{
				r.Texture = _Texture;
				Target.Draw(_ImageVertices, PrimitiveType.Quads, r);
			}

			r.Texture = null;

			if (Unit.Moved) Target.Draw(_MovedRect, PrimitiveType.Quads, r);
			if (Unit.Fired) Target.Draw(_FiredRect, PrimitiveType.Quads, r);
		}
	}
}
