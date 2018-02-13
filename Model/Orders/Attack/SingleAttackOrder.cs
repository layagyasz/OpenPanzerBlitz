using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public abstract class SingleAttackOrder : Order
	{
		public Unit Attacker { get; protected set; }
		public Unit Defender { get; protected set; }
		public bool TreatStackAsArmored { get; set; }

		public abstract AttackFactorCalculation GetAttack();

		public Army Army
		{
			get
			{
				return Attacker.Army;
			}		}

		protected SingleAttackOrder(Unit Attacker, Unit Defender)
		{
			this.Attacker = Attacker;
			this.Defender = Defender;
		}

		public abstract void Serialize(SerializationOutputStream Stream);

		public virtual OrderInvalidReason Validate()
		{
			return OrderInvalidReason.NONE;
		}

		public abstract AttackOrder GenerateNewAttackOrder();
		public abstract bool MatchesTurnComponent(TurnComponent TurnComponent);
		public abstract OrderStatus Execute(Random Random);
	}
}
