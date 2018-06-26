using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitMovementRules : Serializable
	{
		enum Attribute
		{
			BASE_COST,
			IGNORES_ENVIRONMENT_MOVEMENT,
			CANNOT_USE_ROAD_MOVEMENT,
			MOVEMENT_RULES
		};

		public readonly string UniqueKey;

		public readonly int BaseCost;
		public readonly bool IgnoresEnvironmentMovement;
		public readonly bool CannotUseRoadMovement;

		public readonly MovementRule[] MovementRules;

		public MovementRule this[TerrainAttribute Terrain]
		{
			get
			{
				return MovementRules[(int)Terrain];
			}
		}

		public UnitMovementRules(ParseBlock Block)
		{
			UniqueKey = Block.Name;

			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			BaseCost = (int)(attributes[(int)Attribute.BASE_COST] ?? 1);
			IgnoresEnvironmentMovement = (bool)(attributes[(int)Attribute.IGNORES_ENVIRONMENT_MOVEMENT] ?? false);
			CannotUseRoadMovement = (bool)(attributes[(int)Attribute.CANNOT_USE_ROAD_MOVEMENT] ?? false);

			MovementRules = Parse.KeyByEnum<TerrainAttribute, MovementRule>(
				(Dictionary<string, MovementRule>)attributes[(int)Attribute.MOVEMENT_RULES]);
		}

		public UnitMovementRules(SerializationInputStream Stream)
		{
			UniqueKey = Stream.ReadString();

			BaseCost = Stream.ReadInt32();
			IgnoresEnvironmentMovement = Stream.ReadBoolean();
			CannotUseRoadMovement = Stream.ReadBoolean();

			MovementRules = Stream.ReadEnumerable(i => new MovementRule(i)).ToArray();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);

			Stream.Write(BaseCost);
			Stream.Write(IgnoresEnvironmentMovement);
			Stream.Write(CannotUseRoadMovement);

			Stream.Write(MovementRules);
		}
	}
}
