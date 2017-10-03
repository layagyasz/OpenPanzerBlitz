using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ObjectiveSerializer : SerializableAdapter
	{
		public static readonly ObjectiveSerializer Instance = new ObjectiveSerializer();

		public ObjectiveSerializer()
			: base(new Type[]
		{
			typeof(FurthestAdvanceObjective),
			typeof(LineOfFireObjective),
			typeof(UnitsDestroyedObjective),
			typeof(UnitsMatchedObjective),
			typeof(PreventEnemyObjective),
			typeof(RatioObjective),
			typeof(SumObjective)
		})
		{ }
	}
}
