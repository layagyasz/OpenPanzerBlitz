using System;
using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class PlayerClient
	{
		GameController _GameController;
		RPCAdapter _Adapter;
		Match _Match;

		public PlayerClient(
			TCPClient Client, Match Match, GameController GameController)
		{
			_GameController = GameController;
			_Match = Match;
			_Adapter = new RPCAdapter(Client, new Dictionary<Type, Func<RPCRequest, RPCResponse>>()
			{
				{ typeof(ExecuteOrderRequest), i => HandleExecuteOrderRequest((ExecuteOrderRequest)i) }
			});
		}

		RPCResponse HandleExecuteOrderRequest(ExecuteOrderRequest Request)
		{
			return new BooleanResponse(_Match.ExecuteOrder(Request.Order));
		}
	}
}
