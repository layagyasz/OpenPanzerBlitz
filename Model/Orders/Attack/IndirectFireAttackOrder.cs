using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class IndirectFireAttackOrder : AttackOrderBase<IndirectFireSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.INDIRECT_FIRE;
			}
		}

		public override bool ResultPerDefender
		{
			get
			{
				return false;
			}
		}

		public override bool AllowNoFurtherAttacks
		{
			get
			{
				return false;
			}
		}

		bool _Targeted;
		byte _ScatterRoll = 255;
		byte _ScatterDirectionRoll = 255;

		public IndirectFireAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile) { }

		public IndirectFireAttackOrder(IndirectFireAttackOrder Copy)
			: base(Copy)
		{
			_Targeted = Copy._Targeted;
		}

		public IndirectFireAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new IndirectFireSingleAttackOrder(Stream, Objects)).ToList();
			_Targeted = Stream.ReadBoolean();
			_ScatterRoll = Stream.ReadByte();
			_ScatterDirectionRoll = Stream.ReadByte();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(_Attackers);
			Stream.Write(_Targeted);
			Stream.Write(_ScatterRoll);
			Stream.Write(_ScatterDirectionRoll);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.ATTACK;
		}

		void Recalculate(Tile Tile)
		{
			_OddsCalculations.Clear();
			if (_Attackers.Count == 0) return;

			var defenders =
				Tile.Units.Where(i => i.CanBeAttackedBy(Army, AttackMethod) == OrderInvalidReason.NONE).ToList();
			if (defenders.Count == 0) return;
			foreach (var defender in defenders)
			{
				_OddsCalculations.Add(
					new OddsCalculation(
						_Attackers, new Unit[] { defender }, AttackMethod, Tile, CombatResultsTable.OddsClamp));
			}
			// Sync TreatStackAsArmored
			foreach (OddsCalculation odds in _OddsCalculations)
				odds.AttackFactorCalculations.ForEach(i => i.Item1.TreatStackAsArmored = odds.StackArmored);
		}

		protected override void Recalculate()
		{
			Recalculate(TargetTile);
		}

		public override Order CloneIfStateful()
		{
			return new IndirectFireAttackOrder(this);
		}

		public override OrderInvalidReason Validate()
		{
			if (Target != AttackTarget.ALL) return OrderInvalidReason.MUST_ATTACK_ALL;
			return base.Validate();
		}

		public override OrderStatus Execute(Random Random)
		{
			Recalculate();

			if (_Targeted)
			{
				Tile tile = TargetTile;
				var v = Validate();
				if (v == OrderInvalidReason.ATTACK_NO_SPOTTER)
				{
					if (_ScatterRoll == 255) _ScatterRoll = (byte)Random.Next(0, 6);

					if (_ScatterRoll == 5) tile = null;
					else if (_ScatterRoll > 1)
					{
						if (_ScatterDirectionRoll == 255) _ScatterDirectionRoll = (byte)Random.Next(0, 6);
						tile = TargetTile.NeighborTiles[_ScatterDirectionRoll];
					}
					Recalculate(tile);
					return DoExecute(Random);
				}
				if (v != OrderInvalidReason.NONE) return OrderStatus.ILLEGAL;
				return DoExecute(Random);
			}

			if (Validate() == OrderInvalidReason.NONE)
			{
				_Targeted = true;
				_Attackers.ForEach(i => i.Execute(Random));
				return OrderStatus.IN_PROGRESS;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
