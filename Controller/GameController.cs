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

		public void DoTurn(TurnInfo TurnInfo)
		{
			_PlayerControllers[TurnInfo.Army].DoTurn(TurnInfo);
		}

		void HandleTurn(object Sender, StartTurnComponentEventArgs E)
		{
			DoTurn(E.TurnInfo);
		}
	}
}
