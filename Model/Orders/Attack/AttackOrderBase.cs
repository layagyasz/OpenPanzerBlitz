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
		public abstract AttackMethod AttackMethod { get; }
		public AttackTarget Target { get; protected set; }
		public virtual CombatResultsTable CombatResultsTable
		{
			get
			{
				return CombatResultsTable.STANDARD_CRT;
			}
		}

		protected List<T> _Attackers = new List<T>();
		protected List<OddsCalculation> _OddsCalculations = new List<OddsCalculation>();

		CombatResult[] _Results = new CombatResult[0];

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

		protected AttackOrderBase(SerializationInputStream Stream, List<GameObject> Objects)
			: this(
				(Army)Objects[Stream.ReadInt32()],
				(Tile)Objects[Stream.ReadInt32()])
		{
			Target = (AttackTarget)Stream.ReadByte();
			_Results = Stream.ReadEnumerable(i => (CombatResult)Stream.ReadByte()).ToArray();
		}

		public virtual void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Army.Id);
			Stream.Write(TargetTile.Id);
			Stream.Write((byte)Target);
			Stream.Write(_Results, i => Stream.Write((byte)i));		}

		public abstract bool MatchesTurnComponent(TurnComponent TurnComponent);

		public void SetAttackTarget(AttackTarget Target)
		{
			this.Target = Target;
			Recalculate();
			if (OnChanged != null) OnChanged(this, EventArgs.Empty);
		}

		public OrderInvalidReason AddAttacker(SingleAttackOrder AttackOrder)
		{
			return AddAttacker((T)AttackOrder);
		}

		public virtual OrderInvalidReason AddAttacker(T AttackOrder)
		{
			_Attackers.Add(AttackOrder);
			Recalculate();
			if (OnChanged != null) OnChanged(this, EventArgs.Empty);
			return OrderInvalidReason.NONE;
		}

		public void RemoveAttacker(Unit Attacker)
		{
			_Attackers.RemoveAll(i => i.Attacker == Attacker);
			Recalculate();
			if (OnChanged != null) OnChanged(this, EventArgs.Empty);
		}

		void Recalculate()
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
						TargetTile));
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
								TargetTile))
					.ArgMax(i => i.TotalAttack));
			}
			else
			{
				_OddsCalculations.AddRange(
					_Attackers
						.GroupBy(i => i.Defender)
						.Select(i => new OddsCalculation(i, new Unit[] { i.Key }, AttackMethod, TargetTile)));
				_OddsCalculations.Sort((x, y) => x.CompareTo(y));
			}
			// Sync TreatStackAsArmored
			foreach (OddsCalculation odds in _OddsCalculations)
				odds.AttackFactorCalculations.ForEach(i => i.Item1.TreatStackAsArmored = odds.StackArmored);
		}

		public virtual OrderInvalidReason Validate()
		{
			if (_OddsCalculations.Count == 0)
			{
				if (TargetTile.Units.Count() == 0) return OrderInvalidReason.TARGET_EMPTY;
				return TargetTile.Units.First().CanBeAttackedBy(Army, AttackMethod);
			}

			if (Army.HasAttackedTile(TargetTile)) return OrderInvalidReason.TARGET_ALREADY_ATTACKED;
			foreach (SingleAttackOrder order in _Attackers)
			{
				var r = order.Validate();
				if (r != OrderInvalidReason.NONE) return r;
			}
			if (TargetTile.CanBeAttacked(AttackMethod) != OrderInvalidReason.NONE)
				return TargetTile.CanBeAttacked(AttackMethod);

			return OrderInvalidReason.NONE;
		}

		public OrderStatus Execute(Random Random)
		{
			Recalculate();
			if (Validate() != OrderInvalidReason.NONE) return OrderStatus.ILLEGAL;

			if (_Results.Length == 0) _Results = new CombatResult[_OddsCalculations.Count];
			for (int i = 0; i < _OddsCalculations.Count; ++i)
			{
				OddsCalculation c = _OddsCalculations[i];
				if (_Results[i] == CombatResult.NONE)
					_Results[i] = CombatResultsTable.GetCombatResult(c, Random.Next(0, 5));
				foreach (Unit u in c.Defenders) u.HandleCombatResult(_Results[i]);
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
