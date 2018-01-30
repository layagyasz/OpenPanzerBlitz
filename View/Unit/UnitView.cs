using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

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

		Button MOVED_DISPLAY;
		Button FIRED_DISPLAY;
		bool DISPLAYED = false;

		UnitConfigurationRenderer _Renderer;
		UnitConfigurationView _UnitConfigurationView;
		MovementDolly _Movement;

		EventBuffer<EventArgs> _UnitConfigurationChangedBuffer = new EventBuffer<EventArgs>();

		public readonly Unit Unit;
		public readonly bool Reactive;

		public override Vector2f Size
		{
			get
			{
				return _UnitConfigurationView.Size;
			}
		}

		public UnitView(Unit Unit, UnitConfigurationRenderer Renderer, float Scale, bool Reactive)
		{
			this.Unit = Unit;
			this.Reactive = Reactive;

			_Renderer = Renderer;
			_UnitConfigurationView = new UnitConfigurationView(
				Unit.Configuration, Unit.Army.Configuration.Faction, Renderer, Scale);
			Unit.OnConfigurationChange += _UnitConfigurationChangedBuffer.Hook(UpdateConfigurationView).Invoke;

			Vector2f tl = new Vector2f(-.5f, -.15f) * Scale;
			Vector2f tr = new Vector2f(.5f, -.15f) * Scale;
			Vector2f br = new Vector2f(.5f, .15f) * Scale;
			Vector2f bl = new Vector2f(-.5f, .15f) * Scale;

			if (MOVED_DISPLAY == null)
			{
				MOVED_DISPLAY = new Button("overlay-moved-box") { DisplayedString = "MOVED" };
				MOVED_DISPLAY.Position = -.5f * new Vector2f(64, MOVED_DISPLAY.Size.Y);
			}
			if (FIRED_DISPLAY == null)
			{
				FIRED_DISPLAY = new Button("overlay-fired-box") { DisplayedString = "FIRED" };
				FIRED_DISPLAY.Position = -.5f * new Vector2f(64, FIRED_DISPLAY.Size.Y);
			}
		}

		void UpdateConfigurationView(object Sender, EventArgs E)
		{
			_UnitConfigurationView = new UnitConfigurationView(
				Unit.Configuration, Unit.Army.Configuration.Faction, _Renderer, _UnitConfigurationView.Scale);
		}

		public void Move(MovementEventArgs E)
		{
			if (E.Path != null) _Movement = new MovementDolly(this, E.Path, E.Carrier);
			else Position = E.Tile.Center;
		}

		public override bool IsCollision(Vector2f Point)
		{
			return _UnitConfigurationView.IsCollision(Point);
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

			// Display overlays since the text will not show up otherwise.
			if (!DISPLAYED)
			{
				Transform.Scale(1 / 64f, 1 / 64f);
				MOVED_DISPLAY.Update(MouseController, KeyController, DeltaT, Transform);
				FIRED_DISPLAY.Update(MouseController, KeyController, DeltaT, Transform);
				DISPLAYED = true;
			}

			_UnitConfigurationChangedBuffer.DispatchEvents();
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

				if (Unit.Moved || Unit.Fired) Transform.Scale(1 / 64f, 1 / 64f);
				if (Unit.Moved && !Unit.Fired) MOVED_DISPLAY.Draw(Target, Transform);
				if (Unit.Fired) FIRED_DISPLAY.Draw(Target, Transform);
			}
		}
	}
}
