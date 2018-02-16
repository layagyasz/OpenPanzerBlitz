using System;
namespace PanzerBlitz
{
	public class ArmyBuilderController
	{
		public EventHandler OnFinished;

		readonly ArmyBuilder _ArmyBuilder;
		readonly ArmyBuilderScreen _Screen;

		public ArmyBuilderController(ArmyBuilder ArmyBuilder, ArmyBuilderScreen Screen)
		{
			_ArmyBuilder = ArmyBuilder;
			_Screen = Screen;
			_Screen.OnFinished += HandleFinished;
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
