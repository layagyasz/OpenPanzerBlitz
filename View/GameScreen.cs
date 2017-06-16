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
		public readonly MapView MapView;
		public readonly HighlightLayer HighlightLayer = new HighlightLayer();
		public readonly List<ArmyView> ArmyViews;

		private PaneLayer _PaneLayer = new PaneLayer();

		public GameScreen(Vector2f WindowSize, Map Map, IEnumerable<Army> Armies)
		{
			Camera = new Camera(WindowSize, new Vector2f((float)Map.Width, (float)Map.Height) * .5f, 64);
			MapView = new MapView(Map);
			ArmyViews = Armies.Select(i => new ArmyView(i)).ToList();
		}

		public void AddPane(Pane Pane)
		{
			_PaneLayer.Add(Pane);
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
			foreach (ArmyView a in ArmyViews) a.Update(MouseController, KeyController, DeltaT, Transform);

			_PaneLayer.Update(MouseController, KeyController, DeltaT, Transform.Identity);
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			Transform = Camera.GetTransform();

			MapView.Draw(Target, Transform);
			HighlightLayer.Draw(Target, Transform);
			foreach (ArmyView a in ArmyViews) a.Draw(Target, Transform);

			_PaneLayer.Draw(Target, Transform.Identity);
		}
	}
}
