using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CloseAssaultSingleAttackOrder : SingleAttackOrder
	{
		public override Tile AttackTile { get; protected set; }

		public CloseAssaultSingleAttackOrder(Unit Attacker, Tile AttackTile, bool UseSecondaryWeapon)
			: base(Attacker, null, UseSecondaryWeapon)
		{
			this.AttackTile = AttackTile;
		}

		public CloseAssaultSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Tile)Objects[Stream.ReadInt32()], Stream.ReadBoolean()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Attacker.Id);
			Stream.Write(AttackTile.Id);
			Stream.Write(UseSecondaryWeapon);
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == OrderInvalidReason.NONE)
				return new AttackFactorCalculation(
					Attacker, AttackMethod.CLOSE_ASSAULT, TreatStackAsArmored, null, UseSecondaryWeapon);
			return new AttackFactorCalculation(
				0,
				new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override AttackOrder GenerateNewAttackOrder()
		{
			return new CloseAssaultAttackOrder(Army, AttackTile);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.CLOSE_ASSAULT;
		}

		public override OrderInvalidReason Validate()
		{
			if (AttackTile == null) return OrderInvalidReason.ILLEGAL;
			if (Attacker.Position.HexCoordinate.Distance(AttackTile.HexCoordinate) > 1)
				return OrderInvalidReason.TARGET_OUT_OF_RANGE;

			return Attacker.CanAttack(AttackMethod.CLOSE_ASSAULT, TreatStackAsArmored, null, UseSecondaryWeapon);
		}

		public override string ToString()
		{
			return string.Format("[CloseAssaultSingleAttackOrder: Attacker={0}, AttackTile={1}]", Attacker, AttackTile);
		}
	}
}
