using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class ArmyView
	{
		public EventHandler<NewUnitViewEventArgs> OnNewUnitView;

		public readonly Army Army;
		public readonly List<UnitView> UnitViews = new List<UnitView>();

		UnitConfigurationRenderer _Renderer;

		public ArmyView(Army Army, UnitConfigurationRenderer Renderer)
		{
			this.Army = Army;
			_Renderer = Renderer;
			foreach (Unit u in Army.Units) AddUnit(u);
			Army.OnUnitAdded += (sender, e) => AddUnit(e.Unit);
		}

		public void AddUnit(Unit Unit)
		{
			UnitView unitView = new UnitView(Unit, _Renderer, .625f);
			UnitViews.Add(unitView);
			if (OnNewUnitView != null) OnNewUnitView(this, new NewUnitViewEventArgs(unitView));
		}
	}
}
