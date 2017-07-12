using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

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

		MapView _MapView;
		private StackLayer _StackLayer = new StackLayer();
		private PaneLayer _PaneLayer = new PaneLayer();
		private List<Pod> _Items = new List<Pod>();

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
			_MapView = MapView;

			this.ArmyViews = ArmyViews.ToList();
			foreach (ArmyView a in this.ArmyViews) _StackLayer.AddArmyView(a);
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

		public void Update(
			MouseController MouseController,
			KeyController KeyController,
			int DeltaT,
			Transform Transform)
		{
			Camera.Update(MouseController, KeyController, DeltaT);
			Transform = Camera.GetTransform();

			MapView.Update(MouseController, KeyController, DeltaT, Transform);
			HighlightLayer.Update(MouseController, KeyController, DeltaT, Transform);
			_StackLayer.Update(MouseController, KeyController, DeltaT, Transform);

			foreach (Pod p in _Items) p.Update(MouseController, KeyController, DeltaT, Transform.Identity);
			_PaneLayer.Update(MouseController, KeyController, DeltaT, Transform.Identity);
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			Transform = Camera.GetTransform();

			MapView.Draw(Target, Transform);
			HighlightLayer.Draw(Target, Transform);
			_StackLayer.Draw(Target, Transform);

			foreach (Pod p in _Items) p.Draw(Target, Transform.Identity);
			_PaneLayer.Draw(Target, Transform.Identity);
		}
	}
}
