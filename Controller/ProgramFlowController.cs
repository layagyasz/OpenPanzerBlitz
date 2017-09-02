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

		Dictionary<ProgramState, ProgramStateController> _ProgramStateControllers =
			new Dictionary<ProgramState, ProgramStateController>
		{
			{ ProgramState.LANDING, new LandingStateController() },
			{ ProgramState.EDIT, new EditStateController() },
			{ ProgramState.LOCAL_SCENARIO_SELECT, new LocalScenarioSelectStateController() },
			{ ProgramState.MATCH, new MatchStateController() },
			{ ProgramState.MATCH_LOBBY, new MatchLobbyStateController() },
			{ ProgramState.LOG_IN_PLAYER, new LogInPlayerStateController() },
			{ ProgramState.REGISTER_PLAYER, new RegisterPlayerStateController() },
			{ ProgramState.SERVER, new ServerStateController() }
		};

		public ProgramFlowController(Interface Interface)
		{
			this.Interface = Interface;
			this.ProgramContext = new ProgramContext(Interface.WindowBounds[2], Interface.KeyController);

			foreach (ProgramStateController controller in _ProgramStateControllers.Values)
				controller.OnProgramStateTransition += HandleStateChange;
		}

		public void EnterState(ProgramState ProgramState, ProgramStateContext ProgramStateContext)
		{
			_ProgramState = ProgramState;
			Interface.Screen.Clear();
			Interface.Screen.Add(
				_ProgramStateControllers[ProgramState].SetupState(ProgramContext, ProgramStateContext));
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
