using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public abstract class SingleAttackOrder : Order
	{
		public Unit Attacker { get; protected set; }
		public Unit Defender { get; protected set; }
		public abstract Tile AttackTile { get; protected set; }
		public bool TreatStackAsArmored { get; set; }
		public bool UseSecondaryWeapon { get; private set; }

		public abstract AttackFactorCalculation GetAttack();

		public Army Army
		{
			get
			{
				return Attacker.Army;
			}		}

		protected SingleAttackOrder(Unit Attacker, Unit Defender, bool UseSecondaryWeapon)
		{
			this.Attacker = Attacker;
			this.Defender = Defender;
			this.UseSecondaryWeapon = UseSecondaryWeapon;
		}

		public Order CloneIfStateful()
		{
			return this;
		}

		public abstract void Serialize(SerializationOutputStream Stream);

		public abstract OrderInvalidReason Validate();
		public abstract AttackOrder GenerateNewAttackOrder();
		public abstract bool MatchesTurnComponent(TurnComponent TurnComponent);
		public abstract OrderStatus Execute(Random Random);
	}
}
