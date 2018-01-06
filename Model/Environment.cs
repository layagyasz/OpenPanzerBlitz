using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Environment : Serializable
	{
		enum Attribute { TILE_RULE_SET, MOVEMENT_MULTIPLIER, RESTRICT_ROAD_MOVEMENT };

		public readonly string UniqueKey;
		public readonly TileRuleSet TileRuleSet;
		public readonly float MovementMultiplier;

		bool[] _RestrictRoadMovement = new bool[Enum.GetValues(typeof(UnitClass)).Length];

		public Environment(
			string UniqueKey,
			TileRuleSet TileRuleSet,
			float MovementMultiplier,
			IEnumerable<UnitClass> RoadMovementRestricted)
		{
			this.UniqueKey = UniqueKey;
			this.TileRuleSet = TileRuleSet;
			this.MovementMultiplier = MovementMultiplier;

			foreach (UnitClass unitClass in RoadMovementRestricted) _RestrictRoadMovement[(int)unitClass] = true;
		}

		public Environment(SerializationInputStream Stream)
		{
			UniqueKey = Stream.ReadString();
			TileRuleSet = new TileRuleSet(Stream);
			MovementMultiplier = Stream.ReadFloat();
			_RestrictRoadMovement = Stream.ReadEnumerable(i => i.ReadBoolean()).ToArray();
		}

		public Environment(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			TileRuleSet = (TileRuleSet)attributes[(int)Attribute.TILE_RULE_SET];
			MovementMultiplier = Parse.DefaultIfNull(attributes[(int)Attribute.MOVEMENT_MULTIPLIER], 1f);

			foreach (UnitClass unitClass in Parse.DefaultIfNull(
				(List<UnitClass>)attributes[(int)Attribute.RESTRICT_ROAD_MOVEMENT], new List<UnitClass>()))
			{
				_RestrictRoadMovement[(int)unitClass] = true;
			}
		}

		public bool IsRoadMovementRestricted(UnitClass UnitClass)
		{
			return _RestrictRoadMovement[(int)UnitClass];
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(TileRuleSet);
			Stream.Write(MovementMultiplier);
			Stream.Write(_RestrictRoadMovement, i => Stream.Write(i));
		}
	}
}
