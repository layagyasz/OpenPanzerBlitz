using System;

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

		public readonly MovementCost DenseEdge;
		public readonly MovementCost Depressed;
		public readonly MovementCost Downhill;
		public readonly MovementCost Frozen;
		public readonly MovementCost Loose;
		public readonly MovementCost Roaded;
		public readonly MovementCost Rough;
		public readonly MovementCost Sloped;
		public readonly MovementCost Swamp;
		public readonly MovementCost Uphill;
		public readonly MovementCost Water;

		public readonly bool IgnoresEnvironmentMovement;
		public readonly bool CannotUseRoadMovement;

		public UnitMovementRules(ParseBlock Block)
		{
			UniqueKey = Block.Name;

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			DenseEdge = Parse.DefaultIfNull(attributes[(int)Attribute.DENSE_EDGE], default(MovementCost));
			Depressed = Parse.DefaultIfNull(attributes[(int)Attribute.DEPRESSED], default(MovementCost));
			Downhill = Parse.DefaultIfNull(attributes[(int)Attribute.DOWNHILL], default(MovementCost));
			Frozen = Parse.DefaultIfNull(attributes[(int)Attribute.FROZEN], default(MovementCost));
			Loose = Parse.DefaultIfNull(attributes[(int)Attribute.LOOSE], default(MovementCost));
			Roaded = Parse.DefaultIfNull(attributes[(int)Attribute.ROADED], default(MovementCost));
			Rough = Parse.DefaultIfNull(attributes[(int)Attribute.ROUGH], default(MovementCost));
			Sloped = Parse.DefaultIfNull(attributes[(int)Attribute.SLOPED], default(MovementCost));
			Swamp = Parse.DefaultIfNull(attributes[(int)Attribute.SWAMP], default(MovementCost));
			Uphill = Parse.DefaultIfNull(attributes[(int)Attribute.UPHILL], default(MovementCost));
			Water = Parse.DefaultIfNull(attributes[(int)Attribute.WATER], default(MovementCost));

			IgnoresEnvironmentMovement = Parse.DefaultIfNull(
				attributes[(int)Attribute.IGNORES_ENVIRONMENT_MOVEMENT], false);
			CannotUseRoadMovement = Parse.DefaultIfNull(
				attributes[(int)Attribute.CANNOT_USE_ROAD_MOVEMENT], false);
		}

		public UnitMovementRules(SerializationInputStream Stream)
		{
			UniqueKey = Stream.ReadString();

			DenseEdge = new MovementCost(Stream);
			Depressed = new MovementCost(Stream);
			Downhill = new MovementCost(Stream);
			Frozen = new MovementCost(Stream);
			Loose = new MovementCost(Stream);
			Roaded = new MovementCost(Stream);
			Rough = new MovementCost(Stream);
			Sloped = new MovementCost(Stream);
			Swamp = new MovementCost(Stream);
			Uphill = new MovementCost(Stream);
			Water = new MovementCost(Stream);

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
