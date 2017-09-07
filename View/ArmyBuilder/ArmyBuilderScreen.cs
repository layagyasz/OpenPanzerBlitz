using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class ArmyBuilderScreen : ScreenBase
	{
		ScrollCollection<StackView> _AvailableUnits = new ScrollCollection<StackView>("army-builder-select");
		ScrollCollection<HomogenousStackView> _SelectedUnits =
			new ScrollCollection<HomogenousStackView>("army-builder-select");

		public ArmyBuilderScreen(
			Vector2f WindowSize,
			IEnumerable<UnitConfigurationLink> UnitConfigurations,
			Faction Faction,
			UnitConfigurationRenderer Renderer)
			: base(WindowSize)
		{
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
		}
	}
}
