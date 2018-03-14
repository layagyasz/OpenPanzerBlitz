using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AirAttackOrder : AttackOrderBase<AirSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.AIR;
			}
		}

		public override bool ResultPerDefender
		{
			get
			{
				return false;
			}
		}

		public AirAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile) { }

		public AirAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new AirSingleAttackOrder(Stream, Objects)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(_Attackers);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.AIRCRAFT;
		}

		public override OrderInvalidReason Validate()
		{
			if (Target != AttackTarget.ALL) return OrderInvalidReason.MUST_ATTACK_ALL;

			return base.Validate();
		}
	}
}
