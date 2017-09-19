using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class MatchController
	{
		Match _Match;
		Dictionary<Army, MatchPlayerController> _PlayerControllers;

		public MatchController(
			Match Match, Dictionary<Army, MatchPlayerController> PlayerControllers)
		{
			_Match = Match;
			_PlayerControllers = PlayerControllers;

			Match.OnStartPhase += HandleTurn;
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