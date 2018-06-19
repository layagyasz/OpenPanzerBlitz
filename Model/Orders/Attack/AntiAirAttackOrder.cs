using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AntiAirAttackOrder : AttackOrderBase<AntiAirSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.ANTI_AIRCRAFT;
			}
		}

		public override bool ResultPerDefender
		{
			get
			{
				return false;
			}
		}

		public override CombatResultsTable CombatResultsTable
		{
			get
			{
				return CombatResultsTable.AA_CRT;
			}
		}

		public AntiAirAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile)
		{
			SetAttackTarget(AttackTarget.EACH);
		}

		public AntiAirAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new AntiAirSingleAttackOrder(Stream, Objects)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(_Attackers);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.ANTI_AIRCRAFT;
		}

		public override OrderInvalidReason Validate()
		{
			if (Target != AttackTarget.EACH) return OrderInvalidReason.MUST_ATTACK_EACH;

			return base.Validate();
		}
	}
}
