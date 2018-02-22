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

		Dictionary<Army, MatchPlayerController> _OverridePlayerControllers =
			new Dictionary<Army, MatchPlayerController>();

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

		public void OverridePlayerController(Army Army, MatchPlayerController Controller)
		{
			_OverridePlayerControllers.Add(Army, Controller);
		}

		public MatchPlayerController GetOverridePlayerController(Army Army)
		{
			return _OverridePlayerControllers.ContainsKey(Army) ? _OverridePlayerControllers[Army] : null;
		}

		public IEnumerable<Army> GetPlayerControlledArmies()
		{
			if (Army == null) return Match.Armies.Where(i => !_OverridePlayerControllers.ContainsKey(i));
			return Enumerable.Repeat(Army, 1);
		}

		public override string ToString()
		{
			return string.Format("[MatchContext: Match={0}]", Match);
		}
	}
}
