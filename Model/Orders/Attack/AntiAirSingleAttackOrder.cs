using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AntiAirSingleAttackOrder : SingleAttackOrder
	{
		public readonly LineOfSight LineOfSight;

		public override Tile AttackTile { get; protected set; }

		public AntiAirSingleAttackOrder(Unit Attacker, Tile AttackTile, bool UseSecondaryWeapon)
			: base(Attacker, null, UseSecondaryWeapon)
		{
			this.AttackTile = AttackTile;
			LineOfSight = Attacker.GetLineOfSight(AttackTile);
		}

		public AntiAirSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Tile)Objects[Stream.ReadInt32()], Stream.ReadBoolean())
		{
			LineOfSight = new LineOfSight((Tile)Objects[Stream.ReadInt32()], AttackTile);
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Attacker.Id);
			Stream.Write(AttackTile.Id);
			Stream.Write(UseSecondaryWeapon);
			Stream.Write(LineOfSight.Initial.Id);
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == OrderInvalidReason.NONE)
				return new AttackFactorCalculation(
					Attacker, AttackMethod.ANTI_AIRCRAFT, TreatStackAsArmored, LineOfSight, UseSecondaryWeapon);
			return new AttackFactorCalculation(
				0,
				new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override AttackOrder GenerateNewAttackOrder()
		{
			return new AntiAirAttackOrder(Army, AttackTile);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.ATTACK;
		}

		public override OrderInvalidReason Validate()
		{
			if (AttackTile == null) return OrderInvalidReason.ILLEGAL;
			return Attacker.CanAttack(AttackMethod.ANTI_AIRCRAFT, TreatStackAsArmored, LineOfSight, UseSecondaryWeapon);
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
	}
}
