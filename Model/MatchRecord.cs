﻿using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MatchRecord : Serializable
	{
		public readonly Match Match;
		public readonly OrderSerializer OrderSerializer;
		public readonly List<Order> Orders;

		public MatchRecord(Match Match, OrderSerializer OrderSerializer)
		{
			this.Match = Match;
			this.OrderSerializer = OrderSerializer ?? new OrderSerializer(Match);
			Orders = Match.ExecutedOrders.ToList();
		}

		public MatchRecord(SerializationInputStream Stream)
		{
			var scenario = new Scenario(Stream);
			scenario.Rules &= new ScenarioRules(false, true);
			Match = new Match(scenario, null);
			OrderSerializer = new OrderSerializer(Match);
			Orders = Stream.ReadEnumerable(() => OrderSerializer.Deserialize(Stream)).ToList();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Match.Scenario);
			Stream.Write(Orders, i => OrderSerializer.Serialize(i, Stream));
		}
	}
}
