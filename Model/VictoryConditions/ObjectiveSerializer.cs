using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ObjectiveSerializer : SerializableAdapter
	{
		public static readonly ObjectiveSerializer Instance = new ObjectiveSerializer();

		ObjectiveSerializer()
			: base(new Type[]
		{
			typeof(CompositeObjective),
			typeof(FurthestAdvanceObjective),
			typeof(HighestScoreObjective),
			typeof(HighestUniqueScoreObjective),
			typeof(LineOfFireObjective),
			typeof(PointsObjective),
			typeof(PreventEnemyObjective),
			typeof(RatioObjective),
			typeof(SumObjective),
			typeof(TemporalObjective),
			typeof(TilesControlledObjective),
			typeof(TriggerObjective),
			typeof(UnitsMatchedObjective)
		})
		{ }

		public override IEnumerable<Tuple<string, Func<ParseBlock, object>>> GetParsers(params Type[] FilterTypes)
		{
			foreach (var p in base.GetParsers(
				Enumerable.Concat(
					FilterTypes,
					new Type[] { typeof(CompositeObjective) }).ToArray()))
				yield return p;

			yield return new Tuple<string, Func<ParseBlock, object>>(
				"achieve-all", i => new CompositeObjective(i, Aggregators.AND));
			yield return new Tuple<string, Func<ParseBlock, object>>(
				"achieve-any", i => new CompositeObjective(i, Aggregators.OR));
		}
	}
}
