using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MinefieldAttackOrder : AttackOrderBase<MinefieldSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.MINEFIELD;
			}
		}

		public override bool ResultPerDefender
		{
			get
			{
				return true;
			}
		}

		public MinefieldAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile) { }

		public MinefieldAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new MinefieldSingleAttackOrder(Stream, Objects)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(_Attackers);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.MINEFIELD_ATTACK;
		}

		public override OrderInvalidReason Validate()
		{
			if (Target != AttackTarget.ALL) return OrderInvalidReason.MUST_ATTACK_ALL;

			foreach (MinefieldSingleAttackOrder attacker in _Attackers)
			{
				if (attacker.Attacker.Position != TargetTile) return OrderInvalidReason.ILLEGAL;
			}

			return base.Validate();
		}
	}
}
