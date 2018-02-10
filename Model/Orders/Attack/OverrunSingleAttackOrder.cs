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
		}

		public OverrunSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this(new MovementOrder(Stream, Objects), (Tile)Objects[Stream.ReadInt32()]) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_InitialMovement);
			Stream.Write(AttackTile.Id);
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
			OrderInvalidReason r = _InitialMovement.Validate();
			// Unit is not stopping in this tile.  Ignore stack limit.
			if (r != OrderInvalidReason.NONE && r != OrderInvalidReason.UNIT_STACK_LIMIT)
				return r;

			r = _InitialMovement.Unit.CanEnter(ExitTile, true);
			if (r != OrderInvalidReason.NONE) return r;
			r = _InitialMovement.Unit.CanEnter(AttackTile, false, true);
			if (r != OrderInvalidReason.NONE) return r;

			MovementCost distance1 = _InitialMovement.Path.Destination.RulesCalculator.GetMoveCost(
				_InitialMovement.Unit, AttackTile, false, true);
			MovementCost distance2 =
				AttackTile.RulesCalculator.GetMoveCost(_InitialMovement.Unit, ExitTile, false, false);
			if (distance1.UnableReason != OrderInvalidReason.NONE) return distance1.UnableReason;
			if (distance2.UnableReason != OrderInvalidReason.NONE) return distance2.UnableReason;

			_MovementPath = new Path<Tile>(_InitialMovement.Path);
			_MovementPath.Add(AttackTile, distance1.Cost);
			_MovementPath.Add(ExitTile, distance2.Cost);
			if (_MovementPath.Distance > _InitialMovement.Unit.RemainingMovement)
				return OrderInvalidReason.UNIT_NO_MOVE;

			r = _InitialMovement.Unit.CanAttack(AttackMethod.OVERRUN, _TreatStackAsArmored, null);
			if (r != OrderInvalidReason.NONE) return r;

			return base.Validate();
		}

		public override OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Attacker.Fire();
				_InitialMovement.Unit.MoveTo(ExitTile, _MovementPath);
				if (_InitialMovement.Unit.Configuration.InnatelyClearsMines)
				{
					foreach (Unit minefield in ExitTile.Units
							 .Where(i => i.Configuration.UnitClass == UnitClass.MINEFIELD).ToList())
						minefield.HandleCombatResult(CombatResult.DESTROY);
					foreach (Unit minefield in AttackTile.Units
							 .Where(i => i.Configuration.UnitClass == UnitClass.MINEFIELD).ToList())
						minefield.HandleCombatResult(CombatResult.DESTROY);
				}
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}

		public override string ToString()
		{
			return string.Format(
				"[OverrunSingleAttackOrder: Attacker={0}, AttackTile={1}, Movement={2}]",
				Attacker,
				AttackTile,
				_InitialMovement);
		}
	}
}
