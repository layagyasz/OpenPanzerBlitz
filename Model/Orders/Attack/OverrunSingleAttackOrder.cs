using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class OverrunSingleAttackOrder : SingleAttackOrder
	{
		MovementOrder _InitialMovement;
		Path<Tile> _MovementPath;

		Tile _AttackTile;
		Tile _ExitTile;
		NoMoveReason _Validate;
		bool _TreatStackAsArmored;

		public override Unit Attacker
		{
			get
			{
				return _InitialMovement.Unit;
			}
		}

		public override Unit Defender
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

		public OverrunSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this(new MovementOrder(Stream, Objects), (Tile)Objects[Stream.ReadInt32()]) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_InitialMovement);
			Stream.Write(_AttackTile);
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

			_MovementPath = new Path<Tile>(_InitialMovement.Path);
			_MovementPath.Add(_AttackTile, distance1);
			_MovementPath.Add(_ExitTile, distance2);
			if (_MovementPath.Distance > _InitialMovement.Unit.RemainingMovement) return NoMoveReason.NO_MOVE;

			return NoMoveReason.NONE;
		}

		public override void SetTreatStackAsArmored(bool TreatStackAsArmored)
		{
			_TreatStackAsArmored = TreatStackAsArmored;
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == NoSingleAttackReason.NONE)
				return Attacker.Configuration.GetAttack(AttackMethod.OVERRUN, _TreatStackAsArmored, null);
			else return new AttackFactorCalculation(
				0, new List<AttackFactorCalculationFactor>() { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override NoSingleAttackReason Validate()
		{
			if (_Validate != NoMoveReason.NONE) return NoSingleAttackReason.TERRAIN;
			NoSingleAttackReason r = _InitialMovement.Unit.CanAttack(AttackMethod.OVERRUN, _TreatStackAsArmored, null);
			if (r != NoSingleAttackReason.NONE) return r;

			return base.Validate();
		}

		public override bool Execute(Random Random)
		{
			if (Validate() == NoSingleAttackReason.NONE)
			{
				Attacker.Fire();
				_InitialMovement.Unit.MoveTo(_ExitTile, _MovementPath);
				return true;
			}
			else return false;
		}
	}
}
