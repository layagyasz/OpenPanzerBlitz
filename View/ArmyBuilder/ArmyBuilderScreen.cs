using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class ArmyBuilderScreen : ScreenBase
	{
		GuiContainer<Pod> _Pane = new GuiContainer<Pod>("army-builder-pane");

		ArmyParameters _Parameters;
		Faction _Faction;
		IEnumerable<UnitConfigurationLink> _Links;
		Select<UnitClass> _UnitClassSelect = new Select<UnitClass>("army-builder-select");
		UnitConfigurationTable _AvailableUnits;

		public ArmyBuilderScreen(
			Vector2f WindowSize,
			IEnumerable<UnitConfigurationLink> UnitConfigurations,
			ArmyParameters Parameters,
			Faction Faction,
			UnitConfigurationRenderer Renderer)
			: base(WindowSize)
		{
			_Parameters = Parameters;
			_Faction = Faction;
			_Links = UnitConfigurations;

			_Pane.Position = .5f * (WindowSize - _Pane.Size);

			_AvailableUnits = new UnitConfigurationTable(
				"army-builder-table", "army-builder-table-row", "army-builder-table-cell", Faction, Renderer);
			_AvailableUnits.Position = new Vector2f(0, _UnitClassSelect.Size.Y + 16);

			foreach (UnitClass c in Enum.GetValues(typeof(UnitClass)))
				_UnitClassSelect.Add(
					new SelectionOption<UnitClass>("army-builder-select-option")
					{
						DisplayedString = ObjectDescriber.Describe(c),
						Value = c
					});

			_UnitClassSelect.OnChange += FilterUnits;
			FilterUnits();

			_Pane.Add(_AvailableUnits);
			_Pane.Add(_UnitClassSelect);
			_Items.Add(_Pane);
		}

		void FilterUnits(object Sender, EventArgs E)
		{
			FilterUnits();
		}

		void FilterUnits()
		{
			_AvailableUnits.Clear();
			foreach (UnitConfigurationLink link in _Links.OrderBy(i => i.UnitConfiguration.Name))
			{
				if (link.Faction == _Faction
					&& link.UnitConfiguration.UnitClass == _UnitClassSelect.Value.Value
					&& _Parameters.Matches(link))
					_AvailableUnits.Add(link.UnitConfiguration);
			}
		}
	}
}
