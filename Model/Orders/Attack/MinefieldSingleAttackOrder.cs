using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MinefieldSingleAttackOrder : SingleAttackOrder
	{
		public override Tile AttackTile
		{
			get
			{
				return Attacker.Position;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		public MinefieldSingleAttackOrder(Unit Attacker)
			: base(Attacker, null, false) { }

		public MinefieldSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()]) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Attacker.Id);
		}

		public override AttackFactorCalculation GetAttack()
		{
			return new AttackFactorCalculation(
				AttackTile.Units.Where(
					i => i.CanBeAttackedBy(Army, AttackMethod.MINEFIELD) == OrderInvalidReason.NONE
						&& Attacker.HasInteraction<ClearMinefieldInteraction>(j => j.Agent == i) == null)
				.Sum(i => i.Configuration.Defense) * Attacker.Configuration.PrimaryWeapon.Attack,
				new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.MINEFIELD });
		}

		public override AttackOrder GenerateNewAttackOrder()
		{
			return new MinefieldAttackOrder(Army, Attacker.Position);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return true;
		}

		public override OrderInvalidReason Validate()
		{
			if (Attacker.CanAttack(AttackMethod.MINEFIELD) != OrderInvalidReason.NONE)
				return OrderInvalidReason.UNIT_NO_ATTACK;
			return OrderInvalidReason.NONE;
		}

		public override OrderStatus Execute(Random Random)
		{
			return Validate() == OrderInvalidReason.NONE ? OrderStatus.FINISHED : OrderStatus.ILLEGAL;
		}

		public override string ToString()
		{
			return string.Format("[MinefieldSingleAttackOrder: Attacker={0}, Defender={1}]", Attacker, Defender);
		}
	}
}
