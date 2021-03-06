﻿using System;
using System.Linq;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class ScenarioBuilderStateController : PagedProgramStateController
	{
		ScenarioBuilder _ScenarioBuilder;
		ScenarioBuilderController _Controller;

		public ScenarioBuilderStateController() : base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			var defaultParameters =
				new ScenarioParameters(
					1939, GameData.MatchSettings.Values.First(), 8, new Coordinate(33, 33), false);
			_ScenarioBuilder = new ScenarioBuilder(defaultParameters);

			var screen = new ScenarioBuilderScreen(ProgramContext.ScreenResolution, _ScenarioBuilder);
			screen.OnMainMenuButtonClicked += HandleBack;

			_Controller = new ScenarioBuilderController(_ScenarioBuilder, screen);
			_Controller.OnFinished += HandleFinished;

			return screen;
		}

		void HandleFinished(object Sender, EventArgs E)
		{
			OnProgramStateTransition(
				this,
				new ProgramStateTransitionEventArgs(
					ProgramState.BUILD_ARMY, new ScenarioBuilderContext(_ScenarioBuilder)));
		}
	}
}
