using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PanzerBlitz
{
	public class MatchRecordReplayPlayerController : MatchPlayerController
	{
		MatchAdapter _Match;
		Queue<Order> _Orders;

		public MatchRecordReplayPlayerController(MatchAdapter Match, MatchRecord MatchRecord)
		{
			_Match = Match;
			_Orders = new Queue<Order>(MatchRecord.Orders);
		}

		public void DoTurn(Turn Turn)
		{
			Task.Run(() => DoTurnAsync(Turn));
		}

		public void DoTurnAsync(Turn Turn)
		{
			try
			{
				bool didAnything = false;
				while (!(_Orders.Peek() is NextPhaseOrder))
				{
					var o = _Orders.Dequeue();
					_Match.ExecuteOrder(o);
					didAnything |= !(o is ResetOrder);
				}
				if (didAnything) Thread.Sleep(WaitMillis(Turn.TurnInfo.TurnComponent));
				if (_Orders.Count < 2) Thread.Sleep(2500);
				_Match.ExecuteOrder(_Orders.Dequeue());
			}
			catch (Exception e) { Console.WriteLine(e); }
		}

		int WaitMillis(TurnComponent TurnComponent)
		{
			switch (TurnComponent)
			{
				case TurnComponent.WAIT:
				case TurnComponent.RESET:
				case TurnComponent.MINEFIELD_ATTACK: return 0;
				case TurnComponent.ARTILLERY:
				case TurnComponent.ATTACK:
				case TurnComponent.AIRCRAFT:
				case TurnComponent.ANTI_AIRCRAFT: return 1000;
				case TurnComponent.DEPLOYMENT: return 2500;
				default: return 5000;
			}
		}

		public void Unhook() { }
	}
}
