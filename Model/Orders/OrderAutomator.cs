using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class OrderAutomator
	{
		public readonly Match Match;

		Dictionary<TurnInfo, List<Order>> _RecurringOrderBuffer = new Dictionary<TurnInfo, List<Order>>();

		public OrderAutomator(Match Match)
		{
			this.Match = Match;

			Match.Armies.ForEach(
					i => i.Deployments.ForEach(
						j => j.Units.ForEach(
							k => k.OnMove += (sender, e) => j.AutomateMovement(Match, k.Configuration.IsVehicle))));
		}

		public bool AutomateTurn(TurnInfo TurnInfo)
		{
			Match.ExecuteOrder(new ResetOrder(TurnInfo.Army, TurnInfo.TurnComponent == TurnComponent.RESET));

			DoBufferedOrders(TurnInfo);

			switch (TurnInfo.TurnComponent)
			{
				case TurnComponent.ATTACK:
					return !TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.NORMAL_FIRE) == NoSingleAttackReason.NONE);
				case TurnComponent.CLOSE_ASSAULT:
					return !TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.CLOSE_ASSAULT) == NoSingleAttackReason.NONE);
				case TurnComponent.DEPLOYMENT:
					return TurnInfo.Army.Deployments.All(i => i.AutomateDeployment(Match));
				case TurnComponent.MINEFIELD_ATTACK:
					DoMinefieldAttacks(TurnInfo.Army);
					return true;
				case TurnComponent.NON_VEHICLE_MOVEMENT:
					TurnInfo.Army.Deployments.ForEach(i => i.AutomateMovement(Match, false));
					return !TurnInfo.Army.Units.Any(i => i.CanMove(false, false) == NoMoveReason.NONE);
				case TurnComponent.RESET:
					return true;
				case TurnComponent.VEHICLE_COMBAT_MOVEMENT:
					return !TurnInfo.Army.Units.Any(i => i.CanMove(true, true) == NoMoveReason.NONE);
				case TurnComponent.VEHICLE_MOVEMENT:
					TurnInfo.Army.Deployments.ForEach(i => i.AutomateMovement(Match, true));
					return !TurnInfo.Army.Units.Any(i => i.CanMove(true, false) == NoMoveReason.NONE);
			}
			return false;
		}

		public void BufferOrder(Order Order, TurnInfo TurnInfo)
		{
			if (!_RecurringOrderBuffer.ContainsKey(TurnInfo)) _RecurringOrderBuffer.Add(TurnInfo, new List<Order>());

			List<Order> orders = _RecurringOrderBuffer[TurnInfo];
			if (!orders.Contains(Order)) orders.Add(Order);
		}

		void DoBufferedOrders(TurnInfo TurnInfo)
		{
			if (_RecurringOrderBuffer.ContainsKey(TurnInfo))
			{
				List<Order> orders = _RecurringOrderBuffer[TurnInfo];
				_RecurringOrderBuffer.Remove(TurnInfo);
				orders.ForEach(i => Match.ExecuteOrder(i));
			}
		}

		void DoMinefieldAttacks(Army Army)
		{
			foreach (Unit u in Army.Units)
			{
				if (u.Position == null) continue;

				Unit mine = u.Position.Units.FirstOrDefault(i => i.Configuration.UnitClass == UnitClass.MINEFIELD);
				if (mine != null && mine.Position != null)
				{
					foreach (Unit d in mine.Position.Units.Where(i => i != mine))
					{
						AttackOrder order = new AttackOrder(mine.Army, u.Position, AttackMethod.MINEFIELD);
						order.AddAttacker(new MinefieldSingleAttackOrder(mine, d));
						Match.ExecuteOrder(order);
					}
				}

			}
		}
	}
}
