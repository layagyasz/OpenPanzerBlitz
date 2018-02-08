using System;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class ArmyBuilderStateController : PagedProgramStateController
	{
		ArmyBuilderController _Controller;

		public ArmyBuilderStateController() : base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = ProgramStateContext;
			ScenarioBuilderContext context = (ScenarioBuilderContext)_Context;

			ArmyBuilder builder = context.ScenarioBuilder.Armies.First(i => !i.Validate());
			ArmyBuilderScreen screen =
				new ArmyBuilderScreen(
					ProgramContext.ScreenResolution,
					GameData.UnitConfigurationLinks,
					builder.Parameters,
					new UnitConfigurationRenderer(
						GameData.UnitConfigurationLinks.Where(
							i => builder.Parameters.Matches(i)).Select(i => i.UnitConfiguration),
						GameData.UnitRenderDetails,
						128,
						1024,
						ClassLibrary.Instance.GetFont("compacta")));
			screen.OnMainMenuButtonClicked += HandleBack;

			_Controller = new ArmyBuilderController(builder, screen);
			_Controller.OnFinished += HandleFinished;

			return screen;
		}

		void HandleFinished(object Sender, EventArgs E)
		{
			ScenarioBuilder builder = ((ScenarioBuilderContext)_Context).ScenarioBuilder;
			ProgramStateTransitionEventArgs transition = null;
			if (builder.Armies.All(i => i.Validate()))
				transition = new ProgramStateTransitionEventArgs(
					ProgramState.MATCH, new MatchContext(new Match(builder.BuildScenario())));
			else transition = new ProgramStateTransitionEventArgs(ProgramState.BUILD_ARMY, _Context);
			OnProgramStateTransition(this, transition);
		}
	}
}
