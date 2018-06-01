using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class IndirectFireSingleAttackOrder : SingleAttackOrder
	{
		public readonly LineOfSight LineOfSight;

		public override Tile AttackTile { get; protected set; }

		public IndirectFireSingleAttackOrder(Unit Attacker, Tile AttackTile, bool UseSecondaryWeapon)
			: base(Attacker, null, UseSecondaryWeapon)
		{
			this.AttackTile = AttackTile;
			LineOfSight = Attacker.GetLineOfSight(AttackTile);
		}

		public IndirectFireSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base((Unit)Objects[Stream.ReadInt32()], null, Stream.ReadBoolean())
		{
			LineOfSight = new LineOfSight((Tile)Objects[Stream.ReadInt32()], (Tile)Objects[Stream.ReadInt32()]);
			AttackTile = LineOfSight.Final;
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Attacker.Id);
			Stream.Write(UseSecondaryWeapon);
			Stream.Write(LineOfSight.Initial.Id);
			Stream.Write(LineOfSight.Final.Id);
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == OrderInvalidReason.NONE)
				return new AttackFactorCalculation(
					Attacker, AttackMethod.INDIRECT_FIRE, TreatStackAsArmored, LineOfSight, UseSecondaryWeapon);
			return new AttackFactorCalculation(
				0,
				new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override AttackOrder GenerateNewAttackOrder()
		{
			return new IndirectFireAttackOrder(Army, AttackTile);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.ATTACK;
		}

		public override OrderInvalidReason Validate()
		{
			return Attacker.CanAttack(AttackMethod.INDIRECT_FIRE, TreatStackAsArmored, LineOfSight, UseSecondaryWeapon);
		}

		public override OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Attacker.Fire(UseSecondaryWeapon);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}

		public override string ToString()
		{
			return string.Format("[IndirectFireSingleAttackOrder: Attacker={0}, Defender={1}]", Attacker, Defender);
		}
	}
}
