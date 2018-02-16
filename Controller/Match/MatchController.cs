using System.Collections.Generic;

namespace PanzerBlitz
{
	public class MatchController
	{
		readonly Match _Match;
		readonly Dictionary<Army, MatchPlayerController> _PlayerControllers;

		public MatchController(
			Match Match, Dictionary<Army, MatchPlayerController> PlayerControllers)
		{
			_Match = Match;
			_PlayerControllers = PlayerControllers;

			Match.OnStartPhase += HandleTurn;
		}

		public void DoTurn(Turn Turn)
		{
			_PlayerControllers[Turn.TurnInfo.Army].DoTurn(Turn);
		}

		void HandleTurn(object Sender, StartTurnComponentEventArgs E)
		{
			DoTurn(E.Turn);
		}

		public void Unhook()
		{
			foreach (MatchPlayerController controller in _PlayerControllers.Values) controller.Unhook();
		}
	}
}