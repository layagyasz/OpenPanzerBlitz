using System;

namespace PanzerBlitz
{
	public class RemotePlayerController : GamePlayerController
	{
		OrderSerializer _OrderSerializer;

		public RemotePlayerController(Match Match)
		{
			_OrderSerializer = new OrderSerializer(Match.GetGameObjects());
		}

		public void DoTurn(TurnInfo TurnInfo)
		{
		}
	}
}
