using System;
namespace PanzerBlitz
{
	public class StartTurnComponentEventArgs
	{
		public readonly Turn Turn;

		public StartTurnComponentEventArgs(Turn Turn)
		{
			this.Turn = Turn;
		}
	}
}
