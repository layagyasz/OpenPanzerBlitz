using System;

using Cardamom.Interface;
using Cardamom.Planar;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitView : Interactive
	{
		static readonly Vertex[] FiredRect =
		{
			new Vertex(new Vector2f(-.5f, -.15f), Color.Red),
			new Vertex(new Vector2f(.5f, -.15f), Color.Red),
			new Vertex(new Vector2f(.5f, .15f) , Color.Red),
			new Vertex( new Vector2f(-.5f, .15f), Color.Red)
		};
		static readonly Vertex[] MovedRect =
		{
			new Vertex(new Vector2f(-.5f, -.15f), Color.Blue),
			new Vertex(new Vector2f(.5f, -.15f), Color.Blue),
			new Vertex(new Vector2f(.5f, .15f) , Color.Blue),
			new Vertex( new Vector2f(-.5f, .15f), Color.Blue)
		};

		UnitConfigurationRenderer _Renderer;
		UnitConfigurationView _UnitConfigurationView;
		Rectangle _Bounds;
		MovementDolly _Movement;

		public readonly Unit Unit;
		public readonly bool Reactive;

		public override Vector2f Size
		{
			get
			{
				return _Bounds.Size;
			}
		}

		public UnitView(Unit Unit, UnitConfigurationRenderer Renderer, float Scale, bool Reactive)
		{
			this.Unit = Unit;
			this.Reactive = Reactive;
			if (Reactive) Unit.OnMove += HandleMove;

			_Renderer = Renderer;
			_UnitConfigurationView = new UnitConfigurationView(
				Unit.Configuration, Unit.Army.Configuration.Faction, Renderer, Scale);
			Unit.OnConfigurationChange += UpdateConfigurationView;

			Vector2f tl = new Vector2f(-.5f, -.15f) * Scale;
			Vector2f tr = new Vector2f(.5f, -.15f) * Scale;
			Vector2f br = new Vector2f(.5f, .15f) * Scale;
			Vector2f bl = new Vector2f(-.5f, .15f) * Scale;

			_Bounds = new Rectangle(new Vector2f(-.5f, -.5f) * Scale, new Vector2f(1, 1) * Scale);
		}

		void UpdateConfigurationView(object Sender, EventArgs E)
		{
			_UnitConfigurationView = new UnitConfigurationView(
				Unit.Configuration, Unit.Army.Configuration.Faction, _Renderer, _UnitConfigurationView.Scale);
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

			_UnitConfigurationView.Flipped = Unit.Status == UnitStatus.DISRUPTED && Reactive;
			_UnitConfigurationView.Draw(Target, Transform);

			if (Reactive)
			{
				Transform.Scale(_UnitConfigurationView.Scale, _UnitConfigurationView.Scale);
				RenderStates r = new RenderStates(Transform);
				if (Unit.Moved) Target.Draw(MovedRect, PrimitiveType.Quads, r);
				if (Unit.Fired) Target.Draw(FiredRect, PrimitiveType.Quads, r);
			}
		}
	}
}
