using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class GameController
	{
		Match _Match;
		Dictionary<Army, GamePlayerController> _PlayerControllers;

		public GameController(Match Match, Dictionary<Army, GamePlayerController> PlayerControllers)
		{
			_Match = Match;
			_PlayerControllers = PlayerControllers;

			Match.OnStartPhase += HandleTurn;
			Match.OnExecuteOrder += HandleExecuteOrder;
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

		void HandleExecuteOrder(object Sender, ExecuteOrderEventArgs E)
		{
			foreach (GamePlayerController c in _PlayerControllers.Values.Distinct()) c.ExecuteOrder(E.Order);
		}
	}
}
