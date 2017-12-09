using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class AttackOrder : Order
	{
		public readonly Army AttackingArmy;
		public readonly Tile AttackAt;
		public readonly AttackMethod AttackMethod;

		AttackTarget _AttackTarget = AttackTarget.ALL;
		List<SingleAttackOrder> _Attackers = new List<SingleAttackOrder>();
		List<OddsCalculation> _OddsCalculations = new List<OddsCalculation>();

		CombatResult[] _Results = new CombatResult[0];

		public Army Army
		{
			get
			{
				return AttackingArmy;
			}
		}
		public AttackTarget AttackTarget
		{
			get
			{
				return _AttackTarget;
			}
		}
		public IEnumerable<OddsCalculation> OddsCalculations
		{
			get
			{
				return _OddsCalculations;
			}
		}

		public AttackOrder(Army AttackingArmy, Tile AttackAt, AttackMethod AttackMethod)
		{
			this.AttackingArmy = AttackingArmy;
			this.AttackAt = AttackAt;
			this.AttackMethod = AttackMethod;
		}

		public AttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this(
				(Army)Objects[Stream.ReadInt32()],
				(Tile)Objects[Stream.ReadInt32()],
				(AttackMethod)Stream.ReadByte())
		{
			_AttackTarget = (AttackTarget)Stream.ReadByte();
			_Attackers = Stream.ReadEnumerable(i => SingleAttackOrderSerializer.Deserialize(Stream, Objects)).ToList();
			_Results = Stream.ReadEnumerable(i => (CombatResult)Stream.ReadByte()).ToArray();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(AttackingArmy.Id);
			Stream.Write(AttackAt.Id);
			Stream.Write((byte)AttackMethod);
			Stream.Write((byte)_AttackTarget);
			Stream.Write(_Attackers, i => SingleAttackOrderSerializer.Serialize(i, Stream));
			Stream.Write(_Results, i => Stream.Write((byte)i));
		}

		public void SetAttackTarget(AttackTarget AttackTarget)
		{
			_AttackTarget = AttackTarget;
			Recalculate();
		}

		public OrderInvalidReason AddAttacker(SingleAttackOrder AttackOrder)
		{
			if (!_Attackers.Any(i => i.Attacker == AttackOrder.Attacker))
			{
				// OrderInvalidReason canAttack = AttackOrder.Validate();
				// if (canAttack != OrderInvalidReason.NONE) return canAttack;

				_Attackers.Add(AttackOrder);
				Recalculate();
				return OrderInvalidReason.NONE;
			}
			return OrderInvalidReason.UNIT_DUPLICATE;
		}

		public void RemoveAttacker(Unit Attacker)
		{
			_Attackers.RemoveAll(i => i.Attacker == Attacker);
			Recalculate();
		}

		void Recalculate()
		{
			_OddsCalculations.Clear();
			if (_Attackers.Count == 0) return;

			if (AttackTarget == AttackTarget.ALL)
			{
				List<Unit> defenders =
					AttackAt.Units.Where(i => i.CanBeAttackedBy(AttackingArmy) == OrderInvalidReason.NONE).ToList();
				if (defenders.Count == 0) return;

				_OddsCalculations.Add(
					new OddsCalculation(
						_Attackers,
						defenders,
						AttackMethod,
						AttackAt));
			}
			else if (AttackTarget == AttackTarget.WEAKEST)
			{
				List<Unit> defenders =
					AttackAt.Units.Where(i => i.CanBeAttackedBy(AttackingArmy) == OrderInvalidReason.NONE).ToList();
				if (defenders.Count == 0) return;

				_OddsCalculations.Add(
					defenders
						.Select(
							i => new OddsCalculation(
								_Attackers,
								new Unit[] { i },
								AttackMethod,
								AttackAt))
					.ArgMax(i => i.TotalAttack));
			}
			else
			{
				_OddsCalculations.AddRange(
					_Attackers
						.GroupBy(i => i.Defender)
						.Select(i => new OddsCalculation(i, new Unit[] { i.Key }, AttackMethod, AttackAt)));
				_OddsCalculations.Sort(
					(x, y) => x.GetOddsIndex().CompareTo(y.GetOddsIndex()));
			}
			// Sync TreatStackAsArmored
			foreach (OddsCalculation odds in _OddsCalculations)
				odds.AttackFactorCalculations.ForEach(i => i.Item1.SetTreatStackAsArmored(odds.StackArmored));
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			switch (AttackMethod)
			{
				case AttackMethod.CLOSE_ASSAULT: return TurnComponent == TurnComponent.CLOSE_ASSAULT;
				case AttackMethod.MINEFIELD: return TurnComponent == TurnComponent.MINEFIELD_ATTACK;
				case AttackMethod.NORMAL_FIRE: return TurnComponent == TurnComponent.ATTACK;
				case AttackMethod.OVERRUN: return TurnComponent == TurnComponent.VEHICLE_COMBAT_MOVEMENT;
				default: return false;
			}
		}

		public virtual OrderInvalidReason Validate()
		{
			Recalculate();

			if (_OddsCalculations.Count == 0)
			{
				if (AttackAt.Units.Count() == 0) return OrderInvalidReason.TARGET_EMPTY;
				return AttackAt.Units.First().CanBeAttackedBy(AttackingArmy);
			}

			if (AttackingArmy.HasAttackedTile(AttackAt)) return OrderInvalidReason.TARGET_ALREADY_ATTACKED;
			if (MustAttackAllUnits() && AttackTarget != AttackTarget.ALL) return OrderInvalidReason.MUST_ATTACK_ALL;
			foreach (SingleAttackOrder order in _Attackers)
			{
				OrderInvalidReason r = order.Validate();
				if (r != OrderInvalidReason.NONE) return r;
			}
			if (AttackAt.CanBeAttacked(AttackMethod) != OrderInvalidReason.NONE)
				return AttackAt.CanBeAttacked(AttackMethod);
			if (_AttackTarget == AttackTarget.EACH)
			{
				foreach (SingleAttackOrder attacker in _Attackers)
				{
					OrderInvalidReason r = attacker.Defender.CanBeAttackedBy(AttackingArmy);
					if (r != OrderInvalidReason.NONE) return r;
				}
				if (_OddsCalculations.Count != AttackAt.Units.Count(
					i => i.CanBeAttackedBy(AttackingArmy) == OrderInvalidReason.NONE))
					return OrderInvalidReason.ILLEGAL_ATTACK_EACH;
				if (_OddsCalculations.Any(i => i.GetOddsIndex() < 3))
					return OrderInvalidReason.ILLEGAL_ATTACK_EACH;
			}

			if (AttackMethod == AttackMethod.OVERRUN)
			{
				if (!_Attackers.All(i => i is OverrunSingleAttackOrder)) return OrderInvalidReason.ILLEGAL;
				foreach (var g in _Attackers.Cast<OverrunSingleAttackOrder>().GroupBy(i => i.ExitTile))
				{
					if (g.Key.GetStackSize() +
						g.Sum(i => i.ExitTile.Units.Contains(i.Attacker) ? 0 : i.Attacker.GetStackSize())
						> AttackingArmy.Configuration.Faction.StackLimit)
						return OrderInvalidReason.OVERRUN_EXIT;
				}
			}
			else if (AttackMethod == AttackMethod.MINEFIELD)
			{
				if (!_Attackers.All(i => i is MinefieldSingleAttackOrder)) return OrderInvalidReason.ILLEGAL;
			}
			else if (AttackMethod == AttackMethod.CLOSE_ASSAULT)
			{
				if (!_Attackers.All(i => i is NormalSingleAttackOrder)) return OrderInvalidReason.ILLEGAL;
				foreach (SingleAttackOrder attacker in _Attackers)
				{
					if (attacker.Attacker.Configuration.CanOnlySupportCloseAssault
						&& !_Attackers.Any(
							i => i.Attacker.Position == attacker.Attacker.Position
							&& !i.Attacker.Configuration.CanOnlySupportCloseAssault))
						return OrderInvalidReason.UNIT_CLOSE_ASSAULT_SUPPORT;
				}
			}
			else
			{
				if (!_Attackers.All(i => i is NormalSingleAttackOrder)) return OrderInvalidReason.ILLEGAL;
			}

			return OrderInvalidReason.NONE;
		}

		bool MustAttackAllUnits()
		{
			return AttackMethod != AttackMethod.NORMAL_FIRE
											   || AttackAt.RulesCalculator.MustAttackAllUnits
											   || AttackAt.Units.Any(i => i.Configuration.UnitClass == UnitClass.FORT);
		}

		public virtual OrderStatus Execute(Random Random)
		{
			if (Validate() != OrderInvalidReason.NONE) return OrderStatus.ILLEGAL;

			if (_Results.Length == 0) _Results = new CombatResult[_OddsCalculations.Count];
			for (int i = 0; i < _OddsCalculations.Count; ++i)
			{
				OddsCalculation c = _OddsCalculations[i];
				if (_Results[i] == CombatResult.NONE)
					_Results[i] = CombatResultsTable.STANDARD_CRT.GetCombatResult(c, Random.Next(0, 5));
				foreach (Unit u in c.Defenders) u.HandleCombatResult(_Results[i]);
			}
			AttackingArmy.AttackTile(AttackAt);
			_Attackers.ForEach(i => i.Execute(Random));

			return OrderStatus.FINISHED;
		}

		public override string ToString()
		{
			return string.Format(
				"[AttackOrder: Army={0}, AttackTarget={1}, AttackMethod={2}, Attackers={3}]",
				Army,
				AttackTarget,
				AttackMethod,
				string.Join(",", _Attackers.Select(i => i.ToString())));
		}
	}
}
