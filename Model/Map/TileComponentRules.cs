using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileComponentRules
	{
		enum Attribute
		{
			DIE_MODIFIER,
			MOVE_COST,
			TRUCK_MOVE_COST,
			LEAVE_COST,
			TRUCK_LEAVE_COST,
			ROAD_MOVE,
			NO_CROSSING,
			NO_VEHICLE_CROSSING,
			TREAT_UNITS_AS_ARMORED,
			MUST_ATTACK_ALL_UNITS,
			ELEVATED,
			DEPRESSED,
			DEPRESSED_TRANSITION,
			BLOCKS_LINE_OF_SIGHT,
			CONCEALING
		};

		public readonly int DieModifier;
		public readonly float MoveCost;
		public readonly float TruckMoveCost;
		public readonly float LeaveCost;
		public readonly float TruckLeaveCost;
		public readonly bool RoadMove;
		public readonly bool NoCrossing;
		public readonly bool NoVehicleCrossing;
		public readonly bool TreatUnitsAsArmored;
		public readonly bool MustAttackAllUnits;
		public readonly bool Elevated;
		public readonly bool Depressed;
		public readonly bool DepressedTransition;
		public readonly bool BlocksLineOfSight;
		public readonly bool Concealing;

		public TileComponentRules(
			int DieModifier,
			float MoveCost,
			float TruckMoveCost,
			float LeaveCost,
			float TruckLeaveCost,
			bool RoadMove,
			bool NoCrossing,
			bool NoVehicleCrossing,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits,
			bool Elevated,
			bool Depressed,
			bool DepressedTransition,
			bool BlocksLineOfSight,
			bool Concealing)
		{
			this.DieModifier = DieModifier;
			this.MoveCost = MoveCost;
			this.TruckMoveCost = TruckMoveCost;
			this.LeaveCost = LeaveCost;
			this.TruckLeaveCost = TruckLeaveCost;
			this.RoadMove = RoadMove;
			this.NoCrossing = NoCrossing;
			this.NoVehicleCrossing = NoVehicleCrossing;
			this.TreatUnitsAsArmored = TreatUnitsAsArmored;
			this.MustAttackAllUnits = MustAttackAllUnits;
			this.Elevated = Elevated;
			this.Depressed = Depressed;
			this.DepressedTransition = DepressedTransition;
			this.BlocksLineOfSight = BlocksLineOfSight;
			this.Concealing = Concealing;
		}

		public TileComponentRules(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			DieModifier = Parse.DefaultIfNull(attributes[(int)Attribute.DIE_MODIFIER], 0);
			MoveCost = Parse.DefaultIfNull(attributes[(int)Attribute.MOVE_COST], 0f);
			TruckMoveCost = Parse.DefaultIfNull(attributes[(int)Attribute.TRUCK_MOVE_COST], MoveCost);
			LeaveCost = Parse.DefaultIfNull(attributes[(int)Attribute.LEAVE_COST], 0f);
			TruckLeaveCost = Parse.DefaultIfNull(attributes[(int)Attribute.TRUCK_LEAVE_COST], LeaveCost);
			RoadMove = Parse.DefaultIfNull(attributes[(int)Attribute.ROAD_MOVE], false);
			NoCrossing = Parse.DefaultIfNull(attributes[(int)Attribute.NO_CROSSING], false);
			NoVehicleCrossing = Parse.DefaultIfNull(attributes[(int)Attribute.NO_VEHICLE_CROSSING], NoCrossing);
			TreatUnitsAsArmored = Parse.DefaultIfNull(attributes[(int)Attribute.TREAT_UNITS_AS_ARMORED], false);
			MustAttackAllUnits = Parse.DefaultIfNull(attributes[(int)Attribute.MUST_ATTACK_ALL_UNITS], false);
			Elevated = Parse.DefaultIfNull(attributes[(int)Attribute.ELEVATED], false);
			Depressed = Parse.DefaultIfNull(attributes[(int)Attribute.DEPRESSED], false);
			DepressedTransition = Parse.DefaultIfNull(attributes[(int)Attribute.DEPRESSED_TRANSITION], false);
			BlocksLineOfSight = Parse.DefaultIfNull(attributes[(int)Attribute.BLOCKS_LINE_OF_SIGHT], false);
			Concealing = Parse.DefaultIfNull(attributes[(int)Attribute.CONCEALING], false);
		}
	}
}
