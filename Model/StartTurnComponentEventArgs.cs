using System;
namespace PanzerBlitz
{
	public class StartTurnComponentEventArgs
	{
		public readonly TurnComponent TurnComponent;

		public StartTurnComponentEventArgs(TurnComponent TurnComponent)
		{
			this.TurnComponent = TurnComponent;
		}
	}
}
