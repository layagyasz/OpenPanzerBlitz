using System;
using System.Collections.Generic;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class ProgramFlowController
	{
		public readonly Interface Interface;
		public readonly ProgramContext ProgramContext;

		ProgramState _ProgramState;
		ProgramStateController _Controller;

		Dictionary<ProgramState, Type> _ProgramStateControllers =
			new Dictionary<ProgramState, Type>
		{
			{ ProgramState.BUILD_ARMY, typeof(ArmyBuilderStateController) },
			{ ProgramState.BUILD_SCENARIO, typeof(ScenarioBuilderStateController) },
			{ ProgramState.EDIT, typeof(EditStateController) },
			{ ProgramState.LANDING, typeof(LandingStateController) },
			{ ProgramState.LOCAL_MATCH_RECORD_SELECT, typeof(LocalMatchRecordSelectStateController) },
			{ ProgramState.LOCAL_SCENARIO_SELECT, typeof(LocalScenarioSelectStateController) },
			{ ProgramState.LOG_IN_PLAYER, typeof(LogInPlayerStateController) },
			{ ProgramState.MATCH, typeof(MatchStateController) },
			{ ProgramState.MATCH_END, typeof(MatchEndStateController) },
			{ ProgramState.MATCH_LOBBY, typeof(MatchLobbyStateController) },
			{ ProgramState.REGISTER_PLAYER, typeof(RegisterPlayerStateController) },
			{ ProgramState.SERVER, typeof(ServerStateController) }
		};

		public ProgramFlowController(Interface Interface)
		{
			this.Interface = Interface;
			this.ProgramContext = new ProgramContext(Interface.WindowBounds[2], Interface.KeyController);
		}

		public void EnterState(ProgramState ProgramState, ProgramStateContext ProgramStateContext)
		{
			_ProgramState = ProgramState;

			ProgramStateController newController =
				(ProgramStateController)_ProgramStateControllers[ProgramState]
					.GetConstructor(new Type[] { }).Invoke(new object[] { });
			newController.OnProgramStateTransition += HandleStateChange;

			if (_Controller != null) _Controller.OnProgramStateTransition -= HandleStateChange;
			_Controller = newController;

			Interface.Screen.Clear();
			Interface.Screen.Add(newController.SetupState(ProgramContext, ProgramStateContext));
		}

		void HandleStateChange(object Sender, ProgramStateTransitionEventArgs E)
		{
			EnterState(E.TransitionState, E.ProgramStateContext);
		}

		public override string ToString()
		{
			return string.Format(
				"[ProgramFlowController: State={0}, ActiveStateController={1}]",
				_ProgramState,
				_ProgramStateControllers[_ProgramState]);
		}
	}
}
