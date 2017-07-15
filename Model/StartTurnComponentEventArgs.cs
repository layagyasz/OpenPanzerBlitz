using System;
namespace PanzerBlitz
{
	public class StartTurnComponentEventArgs
	{
		public readonly Army Army;
		public readonly TurnComponent TurnComponent;

		public StartTurnComponentEventArgs(Army Army, TurnComponent TurnComponent)
		{
			this.Army = Army;
			this.TurnComponent = TurnComponent;
		}
	}
}
