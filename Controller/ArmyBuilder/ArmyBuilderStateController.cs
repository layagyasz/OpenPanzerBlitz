using System;
using System.Linq;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class ArmyBuilderStateController : PagedProgramStateController
	{
		ArmyBuilderController _Controller;

		public ArmyBuilderStateController() : base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = ProgramStateContext;
			var context = (ScenarioBuilderContext)_Context;

			var builder = context.ScenarioBuilder.Armies.First(i => !i.Validate());
			var renderer =
				new UnitConfigurationRenderer(
					GameData.UnitConfigurationLinks.Values.Where(
						i => builder.Parameters.Matches(i)).Select(i => i.UnitConfiguration),
					GameData.UnitRenderDetails,
					128,
					1024,
					ClassLibrary.Instance.GetFont("compacta"));
			var screen =
				new ArmyBuilderScreen(
					ProgramContext.ScreenResolution,
					GameData.UnitConfigurationLinks.Values,
					builder.Parameters,
					renderer);
			screen.OnMainMenuButtonClicked += HandleBack;

			_Controller = new ArmyBuilderController(builder, screen, renderer);
			_Controller.OnFinished += HandleFinished;

			return screen;
		}

		void HandleFinished(object Sender, EventArgs E)
		{
			ScenarioBuilder builder = ((ScenarioBuilderContext)_Context).ScenarioBuilder;
			ProgramStateTransitionEventArgs transition = null;
			if (builder.Armies.All(i => i.Validate()))
				transition = new ProgramStateTransitionEventArgs(
					ProgramState.MATCH,
					new MatchContext(
						new Match(builder.BuildScenario().MakeStatic(new Random()), new FullOrderAutomater())));
			else transition = new ProgramStateTransitionEventArgs(ProgramState.BUILD_ARMY, _Context);
			OnProgramStateTransition(this, transition);
		}
	}
}
