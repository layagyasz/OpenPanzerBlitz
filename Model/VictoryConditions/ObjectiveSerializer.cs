using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public static class ObjectiveSerializer
	{
		static readonly Type[] OBJECTIVE_TYPES =
		{
			typeof(FurthestAdvanceObjective),
			typeof(LineOfFireObjective),
			typeof(UnitsDestroyedObjective),
			typeof(UnitsMatchedObjective),
			typeof(PreventEnemyObjective),
			typeof(RatioObjective),
			typeof(SumObjective)
		};

		static readonly Func<SerializationInputStream, Objective>[] DESERIALIZERS =
		{
			i => new FurthestAdvanceObjective(i),
			i => new LineOfFireObjective(i),
			i => new UnitsDestroyedObjective(i),
			i => new UnitsMatchedObjective(i),
			i => new PreventEnemyObjective(i),
			i => new RatioObjective(i),
			i => new SumObjective(i)
		};

		public static void Serialize(Objective Objective, SerializationOutputStream Stream)
		{
			Stream.Write((byte)Array.IndexOf(OBJECTIVE_TYPES, Objective.GetType()));
			Stream.Write(Objective);
		}

		public static Objective Deserialize(SerializationInputStream Stream)
		{
			return DESERIALIZERS[Stream.ReadByte()](Stream);
		}
	}
}
