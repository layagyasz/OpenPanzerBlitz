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

		public readonly Tile AttackTile;
		public readonly Tile ExitTile;

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
			this.AttackTile = AttackTile;
			ExitTile = AttackTile.GetOppositeNeighbor(InitialMovement.Path.Destination);

			_Validate = InitialValidate();
		}

		public OverrunSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this(new MovementOrder(Stream, Objects), (Tile)Objects[Stream.ReadInt32()]) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_InitialMovement);
			Stream.Write(AttackTile.Id);
		}

		private NoMoveReason InitialValidate()
		{
			NoMoveReason r = _InitialMovement.Validate();
			if (r != NoMoveReason.NONE) return r;

			NoDeployReason noEnter = _InitialMovement.Unit.CanEnter(ExitTile, true);
			if (noEnter != NoDeployReason.NONE) return EnumConverter.ConvertToNoMoveReason(noEnter);

			float distance1 = _InitialMovement.Path.Destination.Rules.GetMoveCost(
				_InitialMovement.Unit, AttackTile, false, true);
			float distance2 = AttackTile.Rules.GetMoveCost(_InitialMovement.Unit, ExitTile, false, false);
			if (Math.Abs(distance1 - float.MaxValue) < float.Epsilon
				|| Math.Abs(distance2 - float.MaxValue) < float.Epsilon) return NoMoveReason.TERRAIN;

			_MovementPath = new Path<Tile>(_InitialMovement.Path);
			_MovementPath.Add(AttackTile, distance1);
			_MovementPath.Add(ExitTile, distance2);
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
				return new AttackFactorCalculation(Attacker, AttackMethod.OVERRUN, _TreatStackAsArmored, null);
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

		public override OrderStatus Execute(Random Random)
		{
			if (Validate() == NoSingleAttackReason.NONE)
			{
				Attacker.Fire();
				_InitialMovement.Unit.MoveTo(ExitTile, _MovementPath);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
