using System;
namespace PanzerBlitz
{
	public interface GamePlayerController
	{
		void DoTurn(Army Army, TurnComponent TurnComponent);
	}
}
