using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public abstract class AttackOrderBase<T> : AttackOrder where T : SingleAttackOrder
	{
		public EventHandler<EventArgs> OnChanged { get; set; }

		public Army Army { get; }
		public Tile TargetTile { get; }
		public AttackTarget Target { get; protected set; }

		public abstract AttackMethod AttackMethod { get; }
		public abstract bool ResultPerDefender { get; }
		public virtual CombatResultsTable CombatResultsTable
		{
			get
			{
				return CombatResultsTable.STANDARD_CRT;
			}
		}

		protected List<T> _Attackers = new List<T>();
		protected List<OddsCalculation> _OddsCalculations = new List<OddsCalculation>();

		readonly List<Tuple<Unit, CombatResult>> _Results = new List<Tuple<Unit, CombatResult>>();

		public IEnumerable<SingleAttackOrder> Attackers
		{
			get
			{
				return _Attackers;
			}
		}
		public IEnumerable<OddsCalculation> OddsCalculations
		{
			get
			{
				return _OddsCalculations;
			}
		}

		protected AttackOrderBase(Army Army, Tile TargetTile)
		{
			this.Army = Army;
			this.TargetTile = TargetTile;

			Target = AttackTarget.ALL;
		}

		protected AttackOrderBase(AttackOrderBase<T> Copy) : this(Copy.Army, Copy.TargetTile)
		{
			Target = Copy.Target;
			_Attackers = Copy._Attackers.Select(i => (T)i.CloneIfStateful()).ToList();
			_Results = Copy._Results.ToList();
		}

		protected AttackOrderBase(SerializationInputStream Stream, List<GameObject> Objects)
			: this(
				(Army)Objects[Stream.ReadInt32()],
				(Tile)Objects[Stream.ReadInt32()])
		{
			Target = (AttackTarget)Stream.ReadByte();
			_Results =
				Stream.ReadEnumerable(
					i => new Tuple<Unit, CombatResult>(
						(Unit)Objects[Stream.ReadInt32()], (CombatResult)Stream.ReadByte()))
				 .ToList();
		}

		public virtual void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Army.Id);
			Stream.Write(TargetTile.Id);
			Stream.Write((byte)Target);
			Stream.Write(_Results, i =>
			{
				Stream.Write(i.Item1.Id);
				Stream.Write((byte)i.Item2);
			});		}

		public abstract bool MatchesTurnComponent(TurnComponent TurnComponent);

		public void SetAttackTarget(AttackTarget Target)
		{
			this.Target = Target;
			Recalculate();
			if (OnChanged != null) OnChanged(this, EventArgs.Empty);
		}

		public bool IsCompatible(SingleAttackOrder AttackOrder)
		{
			return AttackOrder.GetType() == typeof(T);
		}

		public OrderInvalidReason AddAttacker(SingleAttackOrder AttackOrder)
		{
			return AddAttacker((T)AttackOrder);
		}

		public OrderInvalidReason AddAttacker(T AttackOrder)
		{
			if (!_Attackers.Any(i => i.Attacker == AttackOrder.Attacker))
			{
				_Attackers.Add(AttackOrder);
				Recalculate();
				if (OnChanged != null) OnChanged(this, EventArgs.Empty);
				return OrderInvalidReason.NONE;
			}
			return OrderInvalidReason.UNIT_DUPLICATE;
		}

		public void RemoveAttacker(Unit Attacker)
		{
			_Attackers.RemoveAll(i => i.Attacker == Attacker);
			Recalculate();
			if (OnChanged != null) OnChanged(this, EventArgs.Empty);
		}

		protected virtual void Recalculate()
		{
			_OddsCalculations.Clear();
			if (_Attackers.Count == 0) return;

			if (Target == AttackTarget.ALL)
			{
				var defenders =
					TargetTile.Units.Where(
						i => i.CanBeAttackedBy(Army, AttackMethod) == OrderInvalidReason.NONE).ToList();
				if (defenders.Count == 0) return;

				_OddsCalculations.Add(
					new OddsCalculation(
						_Attackers,
						defenders,
						AttackMethod,
						TargetTile,
						CombatResultsTable.OddsClamp));
			}
			else if (Target == AttackTarget.WEAKEST)
			{
				var defenders =
					TargetTile.Units.Where(
						i => i.CanBeAttackedBy(Army, AttackMethod) == OrderInvalidReason.NONE).ToList();
				if (defenders.Count == 0) return;

				_OddsCalculations.Add(
					defenders
						.Select(
							i => new OddsCalculation(
								_Attackers,
								new Unit[] { i },
								AttackMethod,
							TargetTile,
							CombatResultsTable.OddsClamp))
					.ArgMax(i => i.TotalAttack / i.TotalDefense));
			}
			else
			{
				_OddsCalculations.AddRange(
					_Attackers
						.GroupBy(i => i.Defender)
					.Select(
						i => new OddsCalculation(
							i, new Unit[] { i.Key }, AttackMethod, TargetTile, CombatResultsTable.OddsClamp)));
				_OddsCalculations.Sort((x, y) => x.CompareTo(y));
			}
			// Sync TreatStackAsArmored
			foreach (OddsCalculation odds in _OddsCalculations)
				odds.AttackFactorCalculations.ForEach(i => i.Item1.TreatStackAsArmored = odds.StackArmored);
		}

		public virtual Order CloneIfStateful()
		{
			return this;
		}

		public virtual OrderInvalidReason Validate()
		{
			OrderInvalidReason r;
			if (_OddsCalculations.Count == 0)
			{
				if (TargetTile.Units.Count() == 0) return OrderInvalidReason.TARGET_EMPTY;
				return TargetTile.Units.First().CanBeAttackedBy(Army, AttackMethod);
			}

			if (Army.HasAttackedTile(TargetTile)) return OrderInvalidReason.TARGET_ALREADY_ATTACKED;
			foreach (SingleAttackOrder order in _Attackers)
			{
				r = order.Validate();
				if (r != OrderInvalidReason.NONE) return r;
			}
			r = TargetTile.CanBeAttacked(AttackMethod);
			if (r != OrderInvalidReason.NONE) return r;
			if (_Attackers.Select(i => i.AttackTile).Distinct().Count() > 1)
				return OrderInvalidReason.ILLEGAL;

			return OrderInvalidReason.NONE;
		}

		public virtual OrderStatus Execute(Random Random)
		{
			Recalculate();
			if (Validate() != OrderInvalidReason.NONE) return OrderStatus.ILLEGAL;
			return DoExecute(Random);
		}

		protected OrderStatus DoExecute(Random Random)
		{
			if (_Results.Count == 0)
			{
				foreach (OddsCalculation odds in _OddsCalculations)
				{
					var c = CombatResultsTable.GetCombatResult(odds, Random.Next(0, 5));
					foreach (Unit unit in odds.Defenders)
					{
						_Results.Add(new Tuple<Unit, CombatResult>(unit, c));
						if (ResultPerDefender) c = CombatResultsTable.GetCombatResult(odds, Random.Next(0, 5));
					}
				}
			}
			foreach (var result in _Results)
			{
				result.Item1.HandleCombatResult(result.Item2, AttackMethod, Army);
			}

			Army.AttackTile(TargetTile);
			_Attackers.ForEach(i => i.Execute(Random));

			return OrderStatus.FINISHED;
		}

		public override string ToString()
		{
			return string.Format(
				"[AttackOrder: Army={0}, AttackTarget={1}, AttackMethod={2}, Attackers={3}, Results={4}]",
				Army,
				Target,
				AttackMethod,
				string.Join(",", _Attackers.Select(i => i.ToString())),
				string.Join(",", _Results.Select(i => i.ToString())));
		}
	}
}
