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

		OrderInvalidReason _Validate;
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

		OrderInvalidReason InitialValidate()
		{
			OrderInvalidReason r = _InitialMovement.Validate();
			// Unit is not stopping in this tile.  Ignore stack limit.
			if (r != OrderInvalidReason.NONE && r != OrderInvalidReason.UNIT_STACK_LIMIT)
				return r;

			r = _InitialMovement.Unit.CanEnter(ExitTile, true);
			if (r != OrderInvalidReason.NONE) return r;

			float distance1 = _InitialMovement.Path.Destination.RulesCalculator.GetMoveCost(
				_InitialMovement.Unit, AttackTile, false, true);
			float distance2 = AttackTile.RulesCalculator.GetMoveCost(_InitialMovement.Unit, ExitTile, false, false);
			if (Math.Abs(distance1 - float.MaxValue) < float.Epsilon
				|| Math.Abs(distance2 - float.MaxValue) < float.Epsilon) return OrderInvalidReason.MOVEMENT_TERRAIN;

			_MovementPath = new Path<Tile>(_InitialMovement.Path);
			_MovementPath.Add(AttackTile, distance1);
			_MovementPath.Add(ExitTile, distance2);
			if (_MovementPath.Distance > _InitialMovement.Unit.RemainingMovement) return OrderInvalidReason.UNIT_NO_MOVE;

			return OrderInvalidReason.NONE;
		}

		public override void SetTreatStackAsArmored(bool TreatStackAsArmored)
		{
			_TreatStackAsArmored = TreatStackAsArmored;
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == OrderInvalidReason.NONE)
				return new AttackFactorCalculation(Attacker, AttackMethod.OVERRUN, _TreatStackAsArmored, null);
			return new AttackFactorCalculation(
				0, new List<AttackFactorCalculationFactor>() { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return true;
		}

		public override OrderInvalidReason Validate()
		{
			if (_Validate != OrderInvalidReason.NONE) return _Validate;
			OrderInvalidReason r = _InitialMovement.Unit.CanAttack(AttackMethod.OVERRUN, _TreatStackAsArmored, null);
			if (r != OrderInvalidReason.NONE) return r;

			return base.Validate();
		}

		public override OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Attacker.Fire();
				_InitialMovement.Unit.MoveTo(ExitTile, _MovementPath);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
