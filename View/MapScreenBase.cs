using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using Cence;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MapScreenBase : Pod
	{
		public EventHandler<EventArgs> OnPulse;

		public readonly Camera Camera;
		public MapView MapView { get; private set; }
		public readonly HighlightLayer HighlightLayer = new HighlightLayer();
		public readonly Vector2f Size;
		public readonly PaneLayer PaneLayer = new PaneLayer();

		protected Vertex[] _Backdrop;
		protected TileRenderer _TileRenderer;
		protected List<Pod> _TransformedItems = new List<Pod>();
		protected List<Pod> _Items = new List<Pod>();
		protected AlertText _AlertText = new AlertText(2500);

		public MapScreenBase(Vector2f WindowSize, Map Map, TileRenderer TileRenderer)
		{
			MapView = new MapView(Map, TileRenderer);
			_TileRenderer = TileRenderer;

			Size = WindowSize;
			Camera = new Camera(WindowSize, new Vector2f(MapView.Map.Width, MapView.Map.Height) * .5f, 64);

			var backdropColor = MakeBackdropColor(MapView.TileRenderer.BaseColor);
			_Backdrop = new Vertex[]
			{
				new Vertex(new Vector2f(0, 0), backdropColor),
				new Vertex(new Vector2f(WindowSize.X, 0), backdropColor),
				new Vertex(WindowSize, backdropColor),
				new Vertex(new Vector2f(0, WindowSize.Y), backdropColor)
			};
			_AlertText.Position = new Vector2f(.5f * WindowSize.X, 0);
			_TransformedItems.Add(MapView);
			_TransformedItems.Add(HighlightLayer);
			_Items.Add(_AlertText);
		}

		Color MakeBackdropColor(Color BaseColor)
		{
			var f = new FloatingColor(BaseColor).MakeHSL();
			f.B = Math.Min(.2f, Math.Max(0, f.B - .2f));
			f.G = Math.Min(.2f, Math.Max(0, f.G - .2f));
			return f.MakeRGB().ConvertToColor();
		}

		public void SetMap(Map Map)
		{
			_TransformedItems.Remove(MapView);
			MapView = new MapView(Map, _TileRenderer);
			_TransformedItems.Insert(0, MapView);
		}

		public void Alert(string Alert)
		{
			_AlertText.Alert(Alert);
		}

		public virtual void Update(
			MouseController MouseController,
			KeyController KeyController,
			int DeltaT,
			Transform Transform)
		{
			if (OnPulse != null) OnPulse(this, EventArgs.Empty);

			Camera.Update(MouseController, KeyController, DeltaT, PaneLayer.Any(i => i.Hover));
			Transform = Camera.GetTransform();

			foreach (Pod p in _TransformedItems) p.Update(MouseController, KeyController, DeltaT, Transform);
			foreach (Pod p in _Items) p.Update(MouseController, KeyController, DeltaT, Transform.Identity);
			PaneLayer.Update(MouseController, KeyController, DeltaT, Transform.Identity);
		}

		public virtual void Draw(RenderTarget Target, Transform Transform)
		{
			Transform = Camera.GetTransform();

			Target.Draw(_Backdrop, PrimitiveType.Quads);
			MapView.Draw(Target, Transform);
			HighlightLayer.Draw(Target, Transform);

			foreach (Pod p in _TransformedItems) p.Draw(Target, Transform);
			foreach (Pod p in _Items) p.Draw(Target, Transform.Identity);
			PaneLayer.Draw(Target, Transform.Identity);
		}
	}
}
