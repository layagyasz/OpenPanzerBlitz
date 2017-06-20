using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class ArmyView
	{
		public readonly Army Army;
		public readonly List<UnitView> UnitViews;

		private List<UnitView> _DeployedUnits = new List<UnitView>();

		public ArmyView(Army Army, UnitConfigurationRenderer Renderer)
		{
			this.Army = Army;
			UnitViews = Army.Units.Select(i => new UnitView(i, Renderer, .5f)).ToList();
			foreach (UnitView u in UnitViews) u.Unit.OnMove += (s, e) => u.Position = ((Unit)s).Position.Center;
		}

		public void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			foreach (UnitView u in UnitViews.Where(i => i.Unit.Position != null))
			{
				u.Update(MouseController, KeyController, DeltaT, Transform);
			}
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			foreach (UnitView u in UnitViews.Where(i => i.Unit.Position != null))
			{
				u.Draw(Target, Transform);
			}
		}
	}
}
