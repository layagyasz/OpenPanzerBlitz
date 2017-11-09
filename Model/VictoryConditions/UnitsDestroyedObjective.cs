using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitsDestroyedObjective : Objective
	{
		enum Attribute { FRIENDLY, OVERRIDE_SCORES };

		public readonly bool Friendly;
		public readonly Dictionary<UnitConfiguration, int> OverrideScores;

		public UnitsDestroyedObjective(bool Friendly, Dictionary<UnitConfiguration, int> OverrideScores)
		{
			this.Friendly = Friendly;
			this.OverrideScores = OverrideScores;
		}

		public UnitsDestroyedObjective(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Friendly = Parse.DefaultIfNull(attributes[(int)Attribute.FRIENDLY], false);
			if (attributes[(int)Attribute.OVERRIDE_SCORES] != null)
			{
				OverrideScores = ((List<Tuple<object, object>>)attributes[(int)Attribute.OVERRIDE_SCORES])
					.ToDictionary(i => (UnitConfiguration)i.Item1, i => (int)i.Item2);
			}
			else OverrideScores = new Dictionary<UnitConfiguration, int>();
		}

		public UnitsDestroyedObjective(SerializationInputStream Stream)
		{
			Friendly = Stream.ReadBoolean();
			OverrideScores = Stream.ReadEnumerable(
				i => new KeyValuePair<UnitConfiguration, int>(
					GameData.UnitConfigurations[Stream.ReadString()], Stream.ReadInt32()))
								   .ToDictionary(i => i.Key, i => i.Value);
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Friendly);
			Stream.Write(OverrideScores, i =>
			{
				Stream.Write(i.Key.UniqueKey);
				Stream.Write(i.Value);
			});
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			IEnumerable<Unit> countedUnits =
				Match.Armies.Where(i => Friendly == (i.Configuration.Team == ForArmy.Configuration.Team))
					 .SelectMany(i => i.Units)
					 .Where(i => i.Status == UnitStatus.DESTROYED);
			return countedUnits.Sum(
				i => OverrideScores.ContainsKey(i.Configuration) ? OverrideScores[i.Configuration] : 1);
		}
	}
}
