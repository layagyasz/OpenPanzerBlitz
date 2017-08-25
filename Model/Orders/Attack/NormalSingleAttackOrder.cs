using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class NormalSingleAttackOrder : SingleAttackOrder
	{
		public readonly AttackMethod AttackMethod;
		public readonly LineOfSight LineOfSight;

		Unit _Attacker;
		Unit _Defender;
		bool _TreatStackAsArmored;

		public override Unit Attacker
		{
			get
			{
				return _Attacker;
			}
		}

		public override Unit Defender
		{
			get
			{
				return _Defender;
			}
		}

		public NormalSingleAttackOrder(Unit Attacker, Unit Defender, AttackMethod AttackMethod)
		{
			_Attacker = Attacker;
			_Defender = Defender;
			this.AttackMethod = AttackMethod;
			this.LineOfSight = Attacker.GetLineOfSight(Defender.Position);
		}

		public NormalSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this(
				(Unit)Objects[Stream.ReadInt32()],
				(Unit)Objects[Stream.ReadInt32()],
				(AttackMethod)Stream.ReadByte())
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Attacker.Id);
			Stream.Write(Defender.Id);
			Stream.Write((byte)AttackMethod);
		}

		public override void SetTreatStackAsArmored(bool TreatStackAsArmored)
		{
			_TreatStackAsArmored = TreatStackAsArmored;
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == NoSingleAttackReason.NONE)
				return new AttackFactorCalculation(_Attacker, AttackMethod, _TreatStackAsArmored, LineOfSight);
			return new AttackFactorCalculation(
				0, new List<AttackFactorCalculationFactor>() { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override NoSingleAttackReason Validate()
		{
			if (_Defender == null) return NoSingleAttackReason.ILLEGAL;
			NoSingleAttackReason r = _Attacker.CanAttack(AttackMethod, _TreatStackAsArmored, LineOfSight);
			if (r != NoSingleAttackReason.NONE) return r;

			return base.Validate();
		}

		public override bool Execute(Random Random)
		{
			if (Validate() == NoSingleAttackReason.NONE)
			{
				_Attacker.Fire();
				return true;
			}
			return false;
		}
	}
}
