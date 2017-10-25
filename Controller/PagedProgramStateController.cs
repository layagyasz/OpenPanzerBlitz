using System;
namespace PanzerBlitz
{
	public abstract class PagedProgramStateController : ProgramStateController
	{
		ProgramState _DestinationState;

		protected ProgramStateContext _Context;

		protected PagedProgramStateController(ProgramState DestinationState)
		{
			_DestinationState = DestinationState;
		}

		protected void HandleBack(object Sender, EventArgs E)
		{
			if (_Context is NetworkContext)
			{
				((NetworkContext)_Context).Close();
			}
			OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(_DestinationState, null));
		}
	}
}
