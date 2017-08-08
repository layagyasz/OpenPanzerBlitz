using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class GameController
	{
		Dictionary<Army, GamePlayerController> _PlayerControllers;

		public GameController(Match Match, Dictionary<Army, GamePlayerController> PlayerControllers)
		{
			_PlayerControllers = PlayerControllers;
			Match.OnStartPhase += HandleTurn;
			Match.Start();
		}

		void HandleTurn(object Sender, StartTurnComponentEventArgs E)
		{
			_PlayerControllers[E.TurnInfo.Army].DoTurn(E.TurnInfo);
		}
	}
}
