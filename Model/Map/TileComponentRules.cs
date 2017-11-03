using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileComponentRules
	{
		enum Attribute
		{
			DIE_MODIFIER,
			ROAD_MOVE,
			TREAT_UNITS_AS_ARMORED,
			MUST_ATTACK_ALL_UNITS,

			DENSE_EDGE,
			DEPRESSED,
			ELEVATED,
			FROZEN,
			PAVED,
			ROUGH,
			SWAMP,
			WATER,

			DEPRESSED_TRANSITION,
			BLOCKS_LINE_OF_SIGHT,
			CONCEALING
		};

		public readonly int DieModifier;
		public readonly bool RoadMove;
		public readonly bool TreatUnitsAsArmored;
		public readonly bool MustAttackAllUnits;

		public readonly bool DenseEdge;
		public readonly bool Depressed;
		public readonly bool Elevated;
		public readonly bool Frozen;
		public readonly bool Paved;
		public readonly bool Rough;
		public readonly bool Swamp;
		public readonly bool Water;


		public readonly bool DepressedTransition;
		public readonly bool BlocksLineOfSight;
		public readonly bool Concealing;

		public TileComponentRules(
			int DieModifier,
			bool RoadMove,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits,

			bool DenseEdge,
			bool Depressed,
			bool Elevated,
			bool Frozen,
			bool Paved,
			bool Rough,
			bool Swamp,
			bool Water,

			bool DepressedTransition,
			bool BlocksLineOfSight,
			bool Concealing)
		{
			this.DieModifier = DieModifier;
			this.RoadMove = RoadMove;
			this.TreatUnitsAsArmored = TreatUnitsAsArmored;
			this.MustAttackAllUnits = MustAttackAllUnits;

			this.DenseEdge = DenseEdge;
			this.Depressed = Depressed;
			this.Elevated = Elevated;
			this.Frozen = Frozen;
			this.Paved = Paved;
			this.Rough = Rough;
			this.Swamp = Swamp;
			this.Water = Water;

			this.DepressedTransition = DepressedTransition;
			this.BlocksLineOfSight = BlocksLineOfSight;
			this.Concealing = Concealing;
		}

		public TileComponentRules(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			DieModifier = Parse.DefaultIfNull(attributes[(int)Attribute.DIE_MODIFIER], 0);
			RoadMove = Parse.DefaultIfNull(attributes[(int)Attribute.ROAD_MOVE], false);
			TreatUnitsAsArmored = Parse.DefaultIfNull(attributes[(int)Attribute.TREAT_UNITS_AS_ARMORED], false);
			MustAttackAllUnits = Parse.DefaultIfNull(attributes[(int)Attribute.MUST_ATTACK_ALL_UNITS], false);

			DenseEdge = Parse.DefaultIfNull(attributes[(int)Attribute.DENSE_EDGE], false);
			Depressed = Parse.DefaultIfNull(attributes[(int)Attribute.DEPRESSED], false);
			Elevated = Parse.DefaultIfNull(attributes[(int)Attribute.ELEVATED], false);
			Frozen = Parse.DefaultIfNull(attributes[(int)Attribute.FROZEN], false);
			Paved = Parse.DefaultIfNull(attributes[(int)Attribute.PAVED], false);
			Rough = Parse.DefaultIfNull(attributes[(int)Attribute.ROUGH], false);
			Swamp = Parse.DefaultIfNull(attributes[(int)Attribute.SWAMP], false);
			Water = Parse.DefaultIfNull(attributes[(int)Attribute.WATER], false);

			DepressedTransition = Parse.DefaultIfNull(attributes[(int)Attribute.DEPRESSED_TRANSITION], false);
			BlocksLineOfSight = Parse.DefaultIfNull(attributes[(int)Attribute.BLOCKS_LINE_OF_SIGHT], false);
			Concealing = Parse.DefaultIfNull(attributes[(int)Attribute.CONCEALING], false);
		}
	}
}
