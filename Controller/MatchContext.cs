using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class MatchContext : NetworkContext
	{
		public readonly Match Match;
		public readonly OrderSerializer OrderSerializer;
		public readonly Army Army;

		public MatchContext(Match Match)
		{
			this.Match = Match;
		}

		public MatchContext(TCPClient Client, Match Match, OrderSerializer OrderSerializer, Army Army)
			: base(Client)
		{
			this.Match = Match;
			this.OrderSerializer = OrderSerializer;
			this.Army = Army;
		}

		public MatchContext(TCPServer Server, Match Match, OrderSerializer OrderSerializer, Army Army)
			: base(Server)
		{
			this.Match = Match;
			this.OrderSerializer = OrderSerializer;
			this.Army = Army;
		}

		public MatchAdapter MakeMatchAdapter()
		{
			if (IsHost) return new LocalMatchAdapter(Match);
			return new RemoteMatchAdapter(Match, Client, OrderSerializer);
		}

		public IEnumerable<Army> GetArmies()
		{
			if (Client != null || Server != null) return Enumerable.Repeat(Army, 1);
			return Match.Armies;
		}

		public override string ToString()
		{
			return string.Format("[MatchContext: Match={0}]", Match);
		}
	}
}
