using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TemporalObjective : Objective
	{
		enum Attribute { OBJECTIVE }

		public readonly Objective Objective;

		public TemporalObjective(Objective Objective)
		{
			this.Objective = Objective;
		}

		public TemporalObjective(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Objective = (Objective)attributes[(int)Attribute.OBJECTIVE];
		}

		public TemporalObjective(SerializationInputStream Stream)
			: this((Objective)ObjectiveSerializer.Instance.Deserialize(Stream)) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			ObjectiveSerializer.Instance.Serialize(Objective, Stream);
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			int turn = Match.CurrentTurn.TurnNumber == 0 ? Match.Scenario.Turns + 1 : Match.CurrentTurn.TurnNumber;
			return Objective.GetScore(ForArmy, Match, Cache) > 0 ? turn : int.MaxValue;
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Objective.GetTiles(Map);
		}
	}
}
