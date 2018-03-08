using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class NormalSingleAttackOrder : SingleAttackOrder
	{
		public readonly LineOfSight LineOfSight;

		public NormalSingleAttackOrder(Unit Attacker, Unit Defender, bool UseSecondaryWeapon)
			: base(Attacker, Defender, UseSecondaryWeapon)
		{
			LineOfSight = Attacker.GetLineOfSight(Defender.Position);
		}

		public NormalSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base((Unit)Objects[Stream.ReadInt32()], (Unit)Objects[Stream.ReadInt32()], Stream.ReadBoolean())
		{
			LineOfSight = new LineOfSight((Tile)Objects[Stream.ReadInt32()], (Tile)Objects[Stream.ReadInt32()]);
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Attacker.Id);
			Stream.Write(Defender.Id);
			Stream.Write(LineOfSight.Initial.Id);
			Stream.Write(LineOfSight.Final.Id);
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == OrderInvalidReason.NONE)
				return new AttackFactorCalculation(
					Attacker, AttackMethod.NORMAL_FIRE, TreatStackAsArmored, LineOfSight, UseSecondaryWeapon);
			return new AttackFactorCalculation(
				0,
				new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override AttackOrder GenerateNewAttackOrder()
		{
			return new NormalAttackOrder(Army, LineOfSight.Final);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.ATTACK;
		}

		public override OrderInvalidReason Validate()
		{
			if (Defender == null) return OrderInvalidReason.ILLEGAL;
			return Attacker.CanAttack(AttackMethod.NORMAL_FIRE, TreatStackAsArmored, LineOfSight, UseSecondaryWeapon);
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

		public override string ToString()
		{
			return string.Format("[NormalSingleAttackOrder: Attacker={0}, Defender={1}]", Attacker, Defender);
		}
	}
}
