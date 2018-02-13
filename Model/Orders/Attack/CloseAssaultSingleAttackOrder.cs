using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CloseAssaultSingleAttackOrder : SingleAttackOrder
	{
		public CloseAssaultSingleAttackOrder(Unit Attacker, Unit Defender)
			: base(Attacker, Defender) { }

		public CloseAssaultSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Unit)Objects[Stream.ReadInt32()]) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Attacker.Id);
			Stream.Write(Defender.Id);
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == OrderInvalidReason.NONE)
				return new AttackFactorCalculation(Attacker, AttackMethod.CLOSE_ASSAULT, TreatStackAsArmored, null);
			return new AttackFactorCalculation(
				0, new List<AttackFactorCalculationFactor>() { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override AttackOrder GenerateNewAttackOrder()
		{
			return new CloseAssaultAttackOrder(Army, Defender.Position);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.CLOSE_ASSAULT;
		}

		public override OrderInvalidReason Validate()
		{
			if (Defender == null) return OrderInvalidReason.ILLEGAL;
			if (Attacker.Position.HexCoordinate.Distance(Defender.Position.HexCoordinate) > 1)
				return OrderInvalidReason.TARGET_OUT_OF_RANGE;

			OrderInvalidReason r = Attacker.CanAttack(AttackMethod.CLOSE_ASSAULT, TreatStackAsArmored, null);
			if (r != OrderInvalidReason.NONE) return r;
			return base.Validate();
		}

		public override OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Attacker.Fire();
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
