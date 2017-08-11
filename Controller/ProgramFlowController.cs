using System;
using System.Collections.Generic;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class ProgramFlowController
	{
		public readonly Interface Interface;
		public readonly ProgramContext ProgramContext;

		Dictionary<ProgramState, ProgramStateController> _ProgramStateControllers =
			new Dictionary<ProgramState, ProgramStateController>
		{
			{ ProgramState.LANDING, new LandingStateController() },
			{ ProgramState.EDIT, new EditStateController() },
			{ ProgramState.LOCAL_SCENARIO_SELECT, new LocalScenarioSelectStateController() },
			{ ProgramState.LOCAL_MATCH, new LocalMatchStateController() }
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
			Interface.Screen.Clear();
			Interface.Screen.Add(
				_ProgramStateControllers[ProgramState].SetupState(ProgramContext, ProgramStateContext));
		}

		void HandleStateChange(object Sender, ProgramStateTransitionEventArgs E)
		{
			EnterState(E.TransitionState, E.ProgramStateContext);
		}
	}
}
