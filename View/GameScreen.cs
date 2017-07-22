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
	public class GameScreen : Pod
	{
		public readonly Camera Camera;
		public readonly HighlightLayer HighlightLayer = new HighlightLayer();
		public readonly List<ArmyView> ArmyViews;
		public readonly Vector2f Size;

		Vertex[] _Backdrop;
		MapView _MapView;
		private StackLayer _StackLayer = new StackLayer();
		private PaneLayer _PaneLayer = new PaneLayer();
		private List<Pod> _Items = new List<Pod>();
		private AlertText _AlertText = new AlertText(2500);

		public MapView MapView
		{
			get
			{
				return _MapView;
			}
		}

		public GameScreen(Vector2f WindowSize, MapView MapView, IEnumerable<ArmyView> ArmyViews)
		{
			Size = WindowSize;
			Camera = new Camera(
				WindowSize, new Vector2f((float)MapView.Map.Width, (float)MapView.Map.Height) * .5f, 64);

			Color backdropColor = MakeBackdropColor(MapView.TileRenderer.BaseColor);
			_Backdrop = new Vertex[]
			{
				new Vertex(new Vector2f(0, 0), backdropColor),
				new Vertex(new Vector2f(WindowSize.X, 0), backdropColor),
				new Vertex(WindowSize, backdropColor),
				new Vertex(new Vector2f(0, WindowSize.Y), backdropColor)
			};
			_MapView = MapView;
			_AlertText.Position = new Vector2f(.5f * WindowSize.X, 0);

			this.ArmyViews = ArmyViews.ToList();
			foreach (ArmyView a in this.ArmyViews) _StackLayer.AddArmyView(a);
		}

		Color MakeBackdropColor(Color BaseColor)
		{
			FloatingColor f = new FloatingColor(BaseColor).MakeHSL();
			f.B = Math.Min(.2f, Math.Max(0, f.B - .2f));
			f.G = Math.Min(.2f, Math.Max(0, f.G - .2f));
			return f.MakeRGB().ConvertToColor();
		}

		public void SetMapView(MapView MapView)
		{
			_MapView = MapView;
		}

		public void AddItem(Pod Pod)
		{
			_Items.Add(Pod);
		}

		public void AddPane(Pane Pane)
		{
			_PaneLayer.Add(Pane);
		}

		public void RemovePane(Pane Pane)
		{
			_PaneLayer.Remove(Pane);
		}

		public void ClearPanes()
		{
			_PaneLayer.Clear();
		}

		public void Alert(string Alert)
		{
			_AlertText.Alert(Alert);
		}

		public void Update(
			MouseController MouseController,
			KeyController KeyController,
			int DeltaT,
			Transform Transform)
		{
			Camera.Update(MouseController, KeyController, DeltaT, _PaneLayer.Any(i => i.Hover));
			Transform = Camera.GetTransform();

			MapView.Update(MouseController, KeyController, DeltaT, Transform);
			HighlightLayer.Update(MouseController, KeyController, DeltaT, Transform);
			_StackLayer.Update(MouseController, KeyController, DeltaT, Transform);

			foreach (Pod p in _Items) p.Update(MouseController, KeyController, DeltaT, Transform.Identity);
			_AlertText.Update(MouseController, KeyController, DeltaT, Transform.Identity);
			_PaneLayer.Update(MouseController, KeyController, DeltaT, Transform.Identity);
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			Transform = Camera.GetTransform();

			Target.Draw(_Backdrop, PrimitiveType.Quads);
			MapView.Draw(Target, Transform);
			HighlightLayer.Draw(Target, Transform);
			_StackLayer.Draw(Target, Transform);

			foreach (Pod p in _Items) p.Draw(Target, Transform.Identity);
			_AlertText.Draw(Target, Transform.Identity);
			_PaneLayer.Draw(Target, Transform.Identity);
		}
	}
}
