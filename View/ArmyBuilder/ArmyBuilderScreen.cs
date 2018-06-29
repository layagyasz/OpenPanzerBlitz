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
		public EventHandler OnFinished;
		public EventHandler<ValuedEventArgs<UnitConfiguration>> OnUnitConfigurationRightClicked;

		GuiContainer<Pod> _Pane = new GuiContainer<Pod>("army-builder-pane");

		ArmyParameters _Parameters;
		IEnumerable<UnitConfigurationLink> _Links;

		Select<UnitClass> _UnitClassSelect = new Select<UnitClass>("army-builder-select");
		UnitConfigurationTable _AvailableUnits;

		Button _PointTotalButton;
		float _PointTotal;
		UnitConfigurationTable _SelectedUnits;

		Button _Error = new Button("army-builder-error");

		public ArmyBuilderScreen(
			Vector2f WindowSize,
			IEnumerable<UnitConfigurationLink> UnitConfigurations,
			ArmyParameters Parameters,
			UnitConfigurationRenderer Renderer)
			: base(WindowSize)
		{
			_Parameters = Parameters;
			_Links = UnitConfigurations;

			_Pane.Position = .5f * (WindowSize - _Pane.Size);

			var header = new Button("army-builder-header")
			{
				DisplayedString =
					string.Format(
						"{0} - {1} - {2}",
						ObjectDescriber.Describe(Parameters.Faction),
						ObjectDescriber.Describe(Parameters.Parameters.Setting.Front),
						Parameters.Parameters.Year)
			};

			_UnitClassSelect.Position = new Vector2f(0, header.Size.Y);

			_AvailableUnits = new UnitConfigurationTable(
				"army-builder-table",
				"army-builder-table-row",
				"army-builder-table-cell",
				_Parameters.Faction,
				Renderer,
				false);
			_AvailableUnits.Position = new Vector2f(0, _UnitClassSelect.Position.Y + _UnitClassSelect.Size.Y + 16);
			_AvailableUnits.OnUnitClicked += HandleAddUnit;
			_AvailableUnits.OnUnitRightClicked += HandleUnitInfoRequested;

			_PointTotalButton = new Button("army-builder-point-total");
			_PointTotalButton.Position = new Vector2f(_AvailableUnits.Size.X + 16, header.Size.Y);
			SetPointTotal(0);

			_SelectedUnits = new UnitConfigurationTable(
				"army-builder-table",
				"army-builder-table-row",
				"army-builder-table-cell",
				_Parameters.Faction,
				Renderer,
				true);
			_SelectedUnits.Position =
				new Vector2f(_AvailableUnits.Size.X + 16, _PointTotalButton.Position.Y + _PointTotalButton.Size.Y + 16);
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

			var finishedButton = new Button("army-builder-large-button") { DisplayedString = "Finished" };
			finishedButton.Position = _Pane.Size - finishedButton.Size - new Vector2f(32, 32);
			finishedButton.OnClick += HandleFinished;

			_Pane.Add(header);
			_Pane.Add(_AvailableUnits);
			_Pane.Add(_SelectedUnits);
			_Pane.Add(_PointTotalButton);
			_Pane.Add(_UnitClassSelect);
			_Pane.Add(finishedButton);
			_Items.Add(_Pane);
		}

		public void Alert(string Alert)
		{
			_Error.DisplayedString = Alert;
			_Error.Position = new Vector2f(0, _AvailableUnits.Size.Y + _AvailableUnits.Position.Y + 16);
			_Pane.Remove(_Error);
			_Pane.Add(_Error);
		}

		public IEnumerable<Tuple<UnitConfigurationLink, int>> GetSelectedUnits()
		{
			return _SelectedUnits.GetUnitConfigurationLinks();
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
				if ((filterClass == UnitClass.NONE || link.UnitConfiguration.UnitClass == filterClass)
					&& _Parameters.Matches(link))
					_AvailableUnits.Add(link);
			}
		}

		void SetPointTotal(float Total)
		{
			_PointTotal = Total;
			_PointTotalButton.DisplayedString = string.Format("Point Total: {0}/{1}", _PointTotal, _Parameters.Points);
		}

		void HandleAddUnit(object Sender, ValuedEventArgs<UnitConfigurationLink> E)
		{
			_SelectedUnits.Add(E.Value);
			SetPointTotal(_PointTotal + E.Value.UnitConfiguration.GetPointValue(_Parameters.Faction.HalfPriceTrucks));
		}

		void HandleUnitInfoRequested(object Sender, ValuedEventArgs<UnitConfigurationLink> E)
		{
			if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift))
				OnUnitConfigurationRightClicked?.Invoke(
						Sender, new ValuedEventArgs<UnitConfiguration>(E.Value.UnitConfiguration));
		}

		void HandleRemoveUnit(object Sender, ValuedEventArgs<UnitConfigurationLink> E)
		{
			if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift))
				HandleUnitInfoRequested(Sender, E);
			else
			{
				_SelectedUnits.Remove(E.Value);
				SetPointTotal(
					_PointTotal - E.Value.UnitConfiguration.GetPointValue(_Parameters.Faction.HalfPriceTrucks));
			}
		}

		void HandleFinished(object Sender, EventArgs E)
		{
			if (OnFinished != null) OnFinished(this, EventArgs.Empty);
		}
	}
}
