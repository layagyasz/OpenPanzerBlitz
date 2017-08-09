using System;
namespace PanzerBlitz
{
	public class ProgramStateTransitionEventArgs
	{
		public readonly ProgramState TransitionState;
		public readonly ProgramStateContext ProgramStateContext;

		public ProgramStateTransitionEventArgs(ProgramState TransitionState, ProgramStateContext ProgramStateContext)
		{
			this.TransitionState = TransitionState;
			this.ProgramStateContext = ProgramStateContext;
		}
	}
}
