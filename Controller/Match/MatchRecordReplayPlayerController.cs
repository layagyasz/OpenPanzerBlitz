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
					Order o = _Orders.Dequeue();
					_Match.ExecuteOrder(o);
					if (!(o is ResetOrder)) didAnything = true;
				}
				if (didAnything) Thread.Sleep(WaitMillis(Turn.TurnInfo.TurnComponent));
				_Match.ExecuteOrder(_Orders.Dequeue());
			}
			catch (Exception e) { Console.WriteLine(e); }
		}

		int WaitMillis(TurnComponent TurnComponent)
		{
			switch (TurnComponent)
			{
				case TurnComponent.RESET: return 0;
				case TurnComponent.MINEFIELD_ATTACK: return 0;
				case TurnComponent.ATTACK: return 1000;
				case TurnComponent.DEPLOYMENT: return 2500;
				default: return 5000;
			}
		}

		public void Unhook()
		{
		}
	}
}
