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
			typeof(UnitsInZoneObjective),
			typeof(UnitsMatchedObjective),
		};

		static readonly Func<SerializationInputStream, Objective>[] DESERIALIZERS =
		{
			i => new FurthestAdvanceObjective(i),
			i => new LineOfFireObjective(i),
			i => new UnitsDestroyedObjective(i),
			i => new UnitsInZoneObjective(i),
			i => new UnitsMatchedObjective(i)
		};

		public static void Serialize(Objective Objective, SerializationOutputStream Stream)
		{
			Stream.Write((byte)Array.IndexOf(OBJECTIVE_TYPES, Objective.GetType()));
			Stream.Write(Objective);
		}

		public static Objective Deserializer(SerializationInputStream Stream)
		{
			return DESERIALIZERS[Stream.ReadByte()](Stream);
		}
	}
}
