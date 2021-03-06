using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class OverrunSingleAttackOrder : SingleAttackOrder
	{
		readonly MovementOrder _InitialMovement;
		Path<Tile> _MovementPath;

		public readonly Tile ExitTile;

		public override Tile AttackTile { get; protected set; }

		public OverrunSingleAttackOrder(MovementOrder InitialMovement, Tile AttackTile, bool UseSecondaryWeapon)
			: base(InitialMovement.Unit, null, UseSecondaryWeapon)
		{
			_InitialMovement = InitialMovement;
			this.AttackTile = AttackTile;
			ExitTile = AttackTile.GetOppositeNeighbor(InitialMovement.To);
		}

		public OverrunSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this(new MovementOrder(Stream, Objects), (Tile)Objects[Stream.ReadInt32()], Stream.ReadBoolean()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_InitialMovement);
			Stream.Write(AttackTile.Id);
			Stream.Write(UseSecondaryWeapon);
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == OrderInvalidReason.NONE)
				return new AttackFactorCalculation(
					Attacker, AttackMethod.OVERRUN, TreatStackAsArmored, null, UseSecondaryWeapon);
			return new AttackFactorCalculation(
				0,
				new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override AttackOrder GenerateNewAttackOrder()
		{
			return new OverrunAttackOrder(Army, AttackTile);		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return true;
		}

		public override OrderInvalidReason Validate()
		{
			var r = _InitialMovement.Validate();
			// Unit is not stopping in this tile.  Ignore stack limit.
			if (r != OrderInvalidReason.NONE && r != OrderInvalidReason.UNIT_STACK_LIMIT)
				return r;

			r = Attacker.CanEnter(ExitTile, true);
			if (r != OrderInvalidReason.NONE) return r;
			r = Attacker.CanEnter(AttackTile, false, true);
			if (r != OrderInvalidReason.NONE) return r;

			var distance1 =
				_InitialMovement.Path.Destination.Rules.GetMoveCost(_InitialMovement.Unit, AttackTile, false, true);
			var distance2 = AttackTile.Rules.GetMoveCost(_InitialMovement.Unit, ExitTile, false, false);
			if (distance1.UnableReason != OrderInvalidReason.NONE) return distance1.UnableReason;
			if (distance2.UnableReason != OrderInvalidReason.NONE) return distance2.UnableReason;

			_MovementPath = new Path<Tile>(_InitialMovement.Path);
			_MovementPath.Add(AttackTile, distance1.Cost);
			_MovementPath.Add(ExitTile, distance2.Cost);
			if (!Attacker.Configuration.HasUnlimitedMovement() && _MovementPath.Distance > Attacker.RemainingMovement)
				return OrderInvalidReason.UNIT_NO_MOVE;

			return Attacker.CanAttack(AttackMethod.OVERRUN, TreatStackAsArmored, null, UseSecondaryWeapon);
		}

		public override OrderStatus Execute(Random Random)
		{
			Attacker.Fire(AttackTile, UseSecondaryWeapon);
			_InitialMovement.Unit.MoveTo(ExitTile, _MovementPath);
			if (Attacker.Configuration.InnatelyClearsMines)
			{
				foreach (Unit minefield in ExitTile.Units
						 .Where(i => i.Configuration.UnitClass == UnitClass.MINEFIELD).ToList())
					minefield.HandleCombatResult(CombatResult.DESTROY, AttackMethod.NONE, null);
				foreach (Unit minefield in AttackTile.Units
						 .Where(i => i.Configuration.UnitClass == UnitClass.MINEFIELD).ToList())
					minefield.HandleCombatResult(CombatResult.DESTROY, AttackMethod.NONE, null);
			}
			return OrderStatus.FINISHED;
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
