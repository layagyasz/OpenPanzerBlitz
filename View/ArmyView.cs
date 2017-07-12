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

		public ArmyView(Army Army, UnitConfigurationRenderer Renderer)
		{
			this.Army = Army;
			UnitViews = Army.Units.Select(i => new UnitView(i, Renderer, .625f)).ToList();
		}
	}
}
