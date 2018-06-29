using System;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class ArmyBuilderController
	{
		public EventHandler OnFinished;

		readonly ArmyBuilder _ArmyBuilder;
		readonly ArmyBuilderScreen _Screen;

		UnitConfigurationRenderer _Renderer;
		UnitConfigurationInfoPane _Pane;

		public ArmyBuilderController(
			ArmyBuilder ArmyBuilder, ArmyBuilderScreen Screen, UnitConfigurationRenderer Renderer)
		{
			_ArmyBuilder = ArmyBuilder;
			_Screen = Screen;
			_Screen.OnFinished += HandleFinished;
			_Screen.OnUnitConfigurationRightClicked += HandleUnitConfigurationRightClicked;
			_Renderer = Renderer;
		}

		void ClearPane()
		{
			if (_Pane != null) _Screen.PaneLayer.Remove(_Pane);
			_Pane = null;
		}

		void HandleUnitConfigurationRightClicked(object Sender, ValuedEventArgs<UnitConfiguration> E)
		{
			ClearPane();
			_Pane = new UnitConfigurationInfoPane(E.Value, _ArmyBuilder.Parameters.Faction, _Renderer);
			_Pane.OnClose += (sender, e) => ClearPane();
			_Screen.PaneLayer.Add(_Pane);
		}

		void HandleFinished(object Sender, EventArgs E)
		{
			if (_ArmyBuilder.SetUnits(_Screen.GetSelectedUnits()))
			{
				if (OnFinished != null) OnFinished(this, EventArgs.Empty);
			}
			else _Screen.Alert("Too many Points in Units selected.  Please remove some.");
		}
	}
}
