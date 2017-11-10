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
		ScrollCollection<SingularUnitSelectionOption> _AvailableUnits =
			new ScrollCollection<SingularUnitSelectionOption>("army-builder-select");
		ScrollCollection<GroupedUnitSelectionOption> _SelectedUnits =
			new ScrollCollection<GroupedUnitSelectionOption>("army-builder-select");

		public ArmyBuilderScreen(
			Vector2f WindowSize,
			IEnumerable<UnitConfigurationLink> UnitConfigurations,
			Faction Faction,
			UnitConfigurationRenderer Renderer)
			: base(WindowSize)
		{
		}
	}
}
