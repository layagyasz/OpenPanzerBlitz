using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public abstract class ProgramStateController
	{
		public EventHandler<ProgramStateTransitionEventArgs> OnProgramStateTransition;

		public abstract Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext);
	}
}
