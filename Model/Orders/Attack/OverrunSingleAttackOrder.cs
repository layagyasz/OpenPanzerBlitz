using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class OverrunSingleAttackOrder : SingleAttackOrder
	{
		MovementOrder _InitialMovement;
		Tile _AttackTile;
		Tile _ExitTile;
		float _Distance;
		NoMoveReason _Validate;
		bool _TreatStackAsArmored;

		public Unit Attacker
		{
			get
			{
				return _InitialMovement.Unit;
			}
		}

		public Unit Defender
		{
			get
			{
				return null;
			}
		}

		public OverrunSingleAttackOrder(MovementOrder InitialMovement, Tile AttackTile)
		{
			_InitialMovement = InitialMovement;
			_AttackTile = AttackTile;
			_ExitTile = AttackTile.GetOppositeNeighbor(InitialMovement.Path.Destination);

			_Validate = InitialValidate();
		}

		private NoMoveReason InitialValidate()
		{
			NoMoveReason r = _InitialMovement.Validate();
			if (r != NoMoveReason.NONE) return r;

			NoDeployReason noEnter = _InitialMovement.Unit.CanEnter(_ExitTile, true);
			if (noEnter != NoDeployReason.NONE) return EnumConverter.ConvertToNoMoveReason(noEnter);

			float distance1 = _InitialMovement.Path.Destination.Configuration.GetMoveCost(
				_InitialMovement.Unit, _AttackTile, false, true);
			float distance2 = _AttackTile.Configuration.GetMoveCost(_InitialMovement.Unit, _ExitTile, false, false);
			if (Math.Abs(distance1 - float.MaxValue) < float.Epsilon
				|| Math.Abs(distance2 - float.MaxValue) < float.Epsilon) return NoMoveReason.TERRAIN;

			_Distance = (float)_InitialMovement.Path.Distance + distance1 + distance2;
			if (_Distance > _InitialMovement.Unit.RemainingMovement) return NoMoveReason.NO_MOVE;
			return NoMoveReason.NONE;
		}

		public void SetTreatStackAsArmored(bool TreatStackAsArmored)
		{
			_TreatStackAsArmored = TreatStackAsArmored;
		}

		public AttackFactorCalculation GetAttack()
		{
			if (Validate() == NoSingleAttackReason.NONE)
				return Attacker.Configuration.GetAttack(AttackMethod.OVERRUN, _TreatStackAsArmored, null);
			else return new AttackFactorCalculation(
				0, new List<AttackFactorCalculationFactor>() { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public NoSingleAttackReason Validate()
		{
			if (_Validate != NoMoveReason.NONE) return NoSingleAttackReason.TERRAIN;
			NoSingleAttackReason r = _InitialMovement.Unit.CanAttack(AttackMethod.OVERRUN, _TreatStackAsArmored, null);
			if (r != NoSingleAttackReason.NONE) return r;

			return NoSingleAttackReason.NONE;
		}

		public bool Execute(Random Random)
		{
			if (Validate() == NoSingleAttackReason.NONE)
			{
				Attacker.Fire();
				_InitialMovement.Unit.MoveTo(_ExitTile, _Distance);
				return true;
			}
			else return false;
		}
	}
}
