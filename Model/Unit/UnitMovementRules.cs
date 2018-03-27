using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitMovementRules : Serializable
	{
		enum Attribute
		{
			DENSE_EDGE,
			DEPRESSED,
			DOWNHILL,
			FROZEN,
			LEDGE,
			LOOSE,
			ROADED,
			ROUGH,
			SLOPED,
			SWAMP,
			UPHILL,
			WATER,

			IGNORES_ENVIRONMENT_MOVEMENT,
			CANNOT_USE_ROAD_MOVEMENT
		};

		public readonly string UniqueKey;

		public readonly MovementRule DenseEdge;
		public readonly MovementRule Depressed;
		public readonly MovementRule Downhill;
		public readonly MovementRule Frozen;
		public readonly MovementRule Ledge;
		public readonly MovementRule Loose;
		public readonly MovementRule Roaded;
		public readonly MovementRule Rough;
		public readonly MovementRule Sloped;
		public readonly MovementRule Swamp;
		public readonly MovementRule Uphill;
		public readonly MovementRule Water;

		public readonly bool IgnoresEnvironmentMovement;
		public readonly bool CannotUseRoadMovement;

		public UnitMovementRules(ParseBlock Block)
		{
			UniqueKey = Block.Name;

			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			DenseEdge = (MovementRule)(attributes[(int)Attribute.DENSE_EDGE] ?? default(MovementRule));
			Depressed = (MovementRule)(attributes[(int)Attribute.DEPRESSED] ?? default(MovementRule));
			Downhill = (MovementRule)(attributes[(int)Attribute.DOWNHILL] ?? default(MovementRule));
			Frozen = (MovementRule)(attributes[(int)Attribute.FROZEN] ?? default(MovementRule));
			Ledge = (MovementRule)(attributes[(int)Attribute.LEDGE] ?? default(MovementRule));
			Loose = (MovementRule)(attributes[(int)Attribute.LOOSE] ?? default(MovementRule));
			Roaded = (MovementRule)(attributes[(int)Attribute.ROADED] ?? default(MovementRule));
			Rough = (MovementRule)(attributes[(int)Attribute.ROUGH] ?? default(MovementRule));
			Sloped = (MovementRule)(attributes[(int)Attribute.SLOPED] ?? default(MovementRule));
			Swamp = (MovementRule)(attributes[(int)Attribute.SWAMP] ?? default(MovementRule));
			Uphill = (MovementRule)(attributes[(int)Attribute.UPHILL] ?? default(MovementRule));
			Water = (MovementRule)(attributes[(int)Attribute.WATER] ?? default(MovementRule));

			IgnoresEnvironmentMovement = (bool)(attributes[(int)Attribute.IGNORES_ENVIRONMENT_MOVEMENT] ?? false);
			CannotUseRoadMovement = (bool)(attributes[(int)Attribute.CANNOT_USE_ROAD_MOVEMENT] ?? false);
		}

		public UnitMovementRules(SerializationInputStream Stream)
		{
			UniqueKey = Stream.ReadString();

			DenseEdge = new MovementRule(Stream);
			Depressed = new MovementRule(Stream);
			Downhill = new MovementRule(Stream);
			Frozen = new MovementRule(Stream);
			Ledge = new MovementRule(Stream);
			Loose = new MovementRule(Stream);
			Roaded = new MovementRule(Stream);
			Rough = new MovementRule(Stream);
			Sloped = new MovementRule(Stream);
			Swamp = new MovementRule(Stream);
			Uphill = new MovementRule(Stream);
			Water = new MovementRule(Stream);

			IgnoresEnvironmentMovement = Stream.ReadBoolean();
			CannotUseRoadMovement = Stream.ReadBoolean();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);

			Stream.Write(DenseEdge);
			Stream.Write(Depressed);
			Stream.Write(Downhill);
			Stream.Write(Frozen);
			Stream.Write(Ledge);
			Stream.Write(Loose);
			Stream.Write(Roaded);
			Stream.Write(Rough);
			Stream.Write(Sloped);
			Stream.Write(Swamp);
			Stream.Write(Uphill);
			Stream.Write(Water);

			Stream.Write(IgnoresEnvironmentMovement);
			Stream.Write(CannotUseRoadMovement);
		}
	}
}
