using System;
using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class PlayerClient
	{
		RPCAdapter _Adapter;
		GameController _GameController;

		public PlayerClient(TCPConnection Connection, GameController GameController)
		{
			_GameController = GameController;
			_Adapter = new RPCAdapter(Connection, new Dictionary<Type, Func<RPCRequest, RPCResponse>>()
			{
				{ typeof(DoTurnRequest), i => HandleDoTurnRequest((DoTurnRequest)i) }
			});
		}

		RPCResponse HandleDoTurnRequest(DoTurnRequest Request)
		{
			_GameController.DoTurn(Request.TurnInfo);
			return new EmptyResponse();
		}
	}
}
