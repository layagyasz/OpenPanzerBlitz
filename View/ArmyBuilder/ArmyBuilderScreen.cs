using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

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

		Button _PointTotalButton;
		float _PointTotal;
		UnitConfigurationTable _SelectedUnits;

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
				"army-builder-table", "army-builder-table-row", "army-builder-table-cell", Faction, Renderer, false);
			_AvailableUnits.Position = new Vector2f(0, _UnitClassSelect.Size.Y + 16);
			_AvailableUnits.OnUnitClicked += HandleAddUnit;

			_PointTotalButton = new Button("army-builder-point-total");
			_PointTotalButton.Position = new Vector2f(_AvailableUnits.Size.X + 16, 0);
			SetPointTotal(0);

			_SelectedUnits = new UnitConfigurationTable(
				"army-builder-table", "army-builder-table-row", "army-builder-table-cell", Faction, Renderer, true);
			_SelectedUnits.Position = new Vector2f(_AvailableUnits.Size.X + 16, _PointTotalButton.Size.Y + 16);
			_SelectedUnits.OnUnitRightClicked += HandleRemoveUnit;

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
			_Pane.Add(_SelectedUnits);
			_Pane.Add(_PointTotalButton);
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
			UnitClass filterClass = _UnitClassSelect.Value.Value;
			foreach (UnitConfigurationLink link in _Links.OrderBy(i => i.UnitConfiguration.Name))
			{
				if (link.Faction == _Faction
					&& (filterClass == UnitClass.NONE || link.UnitConfiguration.UnitClass == filterClass)
					&& _Parameters.Matches(link))
					_AvailableUnits.Add(link.UnitConfiguration);
			}
		}

		void SetPointTotal(float Total)
		{
			_PointTotal = Total;
			_PointTotalButton.DisplayedString = "Point Total: " + _PointTotal;
		}

		void HandleAddUnit(object Sender, ValuedEventArgs<UnitConfiguration> E)
		{
			_SelectedUnits.Add(E.Value);
			SetPointTotal(E.Value.GetPointValue() + _PointTotal);
		}

		void HandleRemoveUnit(object Sender, ValuedEventArgs<UnitConfiguration> E)
		{
			_SelectedUnits.Remove(E.Value);
			SetPointTotal(E.Value.GetPointValue() - _PointTotal);
		}
	}
}
