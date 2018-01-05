using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileComponentRules : Serializable
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
			LOOSE,
			ROADED,
			ROUGH,
			SWAMP,
			WATER,

			OVERRIDE_BASE_MOVEMENT,
			DEPRESSED_TRANSITION,
			BLOCKS_LINE_OF_SIGHT,
			CONCEALING,
			LOW_PROFILE_CONCEALING
		};

		public readonly string UniqueKey;

		public readonly int DieModifier;
		public readonly bool RoadMove;
		public readonly bool TreatUnitsAsArmored;
		public readonly bool MustAttackAllUnits;

		public readonly bool DenseEdge;
		public readonly bool Depressed;
		public readonly bool Elevated;
		public readonly bool Frozen;
		public readonly bool Loose;
		public readonly bool Roaded;
		public readonly bool Rough;
		public readonly bool Swamp;
		public readonly bool Water;

		public readonly bool OverrideBaseMovement;
		public readonly bool DepressedTransition;
		public readonly bool BlocksLineOfSight;
		public readonly bool Concealing;
		public readonly bool LowProfileConcealing;

		public TileComponentRules(
			string UniqueKey,

			int DieModifier,
			bool RoadMove,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits,

			bool DenseEdge,
			bool Depressed,
			bool Elevated,
			bool Frozen,
			bool Loose,
			bool Roaded,
			bool Rough,
			bool Swamp,
			bool Water,

			bool OverrideBaseMovement,
			bool DepressedTransition,
			bool BlocksLineOfSight,
			bool Concealing,
			bool LowProfileConcealing)
		{
			this.UniqueKey = UniqueKey;

			this.DieModifier = DieModifier;
			this.RoadMove = RoadMove;
			this.TreatUnitsAsArmored = TreatUnitsAsArmored;
			this.MustAttackAllUnits = MustAttackAllUnits;

			this.DenseEdge = DenseEdge;
			this.Depressed = Depressed;
			this.Elevated = Elevated;
			this.Frozen = Frozen;
			this.Loose = Loose;
			this.Roaded = Roaded;
			this.Rough = Rough;
			this.Swamp = Swamp;
			this.Water = Water;

			this.OverrideBaseMovement = OverrideBaseMovement;
			this.DepressedTransition = DepressedTransition;
			this.BlocksLineOfSight = BlocksLineOfSight;
			this.Concealing = Concealing;
			this.LowProfileConcealing = LowProfileConcealing;
		}

		public TileComponentRules(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),

			   	Stream.ReadInt32(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),

				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),

				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean())
		{ }

		public TileComponentRules(ParseBlock Block)
		{
			UniqueKey = Block.Name;

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			DieModifier = Parse.DefaultIfNull(attributes[(int)Attribute.DIE_MODIFIER], 0);
			RoadMove = Parse.DefaultIfNull(attributes[(int)Attribute.ROAD_MOVE], false);
			TreatUnitsAsArmored = Parse.DefaultIfNull(attributes[(int)Attribute.TREAT_UNITS_AS_ARMORED], false);
			MustAttackAllUnits = Parse.DefaultIfNull(attributes[(int)Attribute.MUST_ATTACK_ALL_UNITS], false);

			DenseEdge = Parse.DefaultIfNull(attributes[(int)Attribute.DENSE_EDGE], false);
			Depressed = Parse.DefaultIfNull(attributes[(int)Attribute.DEPRESSED], false);
			Elevated = Parse.DefaultIfNull(attributes[(int)Attribute.ELEVATED], false);
			Frozen = Parse.DefaultIfNull(attributes[(int)Attribute.FROZEN], false);
			Loose = Parse.DefaultIfNull(attributes[(int)Attribute.LOOSE], false);
			Roaded = Parse.DefaultIfNull(attributes[(int)Attribute.ROADED], false);
			Rough = Parse.DefaultIfNull(attributes[(int)Attribute.ROUGH], false);
			Swamp = Parse.DefaultIfNull(attributes[(int)Attribute.SWAMP], false);
			Water = Parse.DefaultIfNull(attributes[(int)Attribute.WATER], false);

			OverrideBaseMovement = Parse.DefaultIfNull(attributes[(int)Attribute.OVERRIDE_BASE_MOVEMENT], true);
			DepressedTransition = Parse.DefaultIfNull(attributes[(int)Attribute.DEPRESSED_TRANSITION], false);
			BlocksLineOfSight = Parse.DefaultIfNull(attributes[(int)Attribute.BLOCKS_LINE_OF_SIGHT], false);
			Concealing = Parse.DefaultIfNull(attributes[(int)Attribute.CONCEALING], false);
			LowProfileConcealing = Parse.DefaultIfNull(attributes[(int)Attribute.LOW_PROFILE_CONCEALING], false);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);

			Stream.Write(DieModifier);
			Stream.Write(RoadMove);
			Stream.Write(TreatUnitsAsArmored);
			Stream.Write(MustAttackAllUnits);

			Stream.Write(DenseEdge);
			Stream.Write(Depressed);
			Stream.Write(Elevated);
			Stream.Write(Frozen);
			Stream.Write(Loose);
			Stream.Write(Roaded);
			Stream.Write(Rough);
			Stream.Write(Swamp);
			Stream.Write(Water);

			Stream.Write(OverrideBaseMovement);
			Stream.Write(DepressedTransition);
			Stream.Write(BlocksLineOfSight);
			Stream.Write(Concealing);
			Stream.Write(LowProfileConcealing);
		}
	}
}
