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
		UnitConfigurationTable _AvailableUnits;

		public ArmyBuilderScreen(
			Vector2f WindowSize,
			IEnumerable<UnitConfigurationLink> UnitConfigurations,
			Faction Faction,
			UnitConfigurationRenderer Renderer)
			: base(WindowSize)
		{
			_AvailableUnits = new UnitConfigurationTable(
				"army-builder-table", "army-builder-table-row", "army-builder-table-cell", Faction, Renderer);
			_AvailableUnits.Position = .5f * (WindowSize - _AvailableUnits.Size);

			foreach (UnitConfigurationLink link in UnitConfigurations.OrderBy(i => i.UnitConfiguration.Name))
			{
				if (link.Faction == Faction) _AvailableUnits.Add(link.UnitConfiguration);
			}
			_Items.Add(_AvailableUnits);
		}
	}
}
