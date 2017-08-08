using System;
namespace PanzerBlitz
{
	public class StartTurnComponentEventArgs
	{
		public readonly TurnInfo TurnInfo;

		public StartTurnComponentEventArgs(TurnInfo TurnInfo)
		{
			this.TurnInfo = TurnInfo;
		}
	}
}
