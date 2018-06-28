﻿using System.Linq;

namespace PanzerBlitz
{
	public class FullOrderAutomater : OrderAutomater
	{
		OrderAutomater _MultiTurnAutomater = new MultiTurnOrderAutomater();

		public void Hook(EventRelay Relay)
		{
			Relay.OnUnitMove += HandleUnitMove;
		}

		void HandleUnitMove(object Sender, MovementEventArgs E)
		{
			var unit = (Unit)Sender;
			unit.Deployment?.EnterUnits(unit.Configuration.IsVehicle);
		}

		public bool AutomateTurn(Match Match, TurnInfo TurnInfo)
		{
			Match.ExecuteOrder(new ResetOrder(TurnInfo.Army, TurnInfo.TurnComponent == TurnComponent.RESET));

			_MultiTurnAutomater.AutomateTurn(Match, TurnInfo);

			switch (TurnInfo.TurnComponent)
			{
				case TurnComponent.DEPLOYMENT:
					TurnInfo.Army.Deployments.ForEach(i => i.AutomateDeployment());
					return false;

				case TurnComponent.MINEFIELD_ATTACK:
					DoMinefieldAttacks(Match, TurnInfo.Army);
					return true;
				case TurnComponent.ARTILLERY:
					return !TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.INDIRECT_FIRE) == OrderInvalidReason.NONE);
				case TurnComponent.ATTACK:
					return !TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.DIRECT_FIRE) == OrderInvalidReason.NONE);

				case TurnComponent.AIRCRAFT:
					return !TurnInfo.Army.Units.Any(
						i => i.Configuration.IsAircraft() && i.Status == UnitStatus.ACTIVE);
				case TurnComponent.ANTI_AIRCRAFT:
					return !TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.ANTI_AIRCRAFT) == OrderInvalidReason.NONE)
						|| !Match.Armies.Where(i => i.Configuration.Team != TurnInfo.Army.Configuration.Team)
						.SelectMany(i => i.Units)
						.Any(i => i.Configuration.IsAircraft() && i.Position != null);

				case TurnComponent.VEHICLE_COMBAT_MOVEMENT:
					return !TurnInfo.Army.Units.Any(i => i.CanMove(true, true) == OrderInvalidReason.NONE
													&& i.CanAttack(AttackMethod.OVERRUN) == OrderInvalidReason.NONE);
				case TurnComponent.VEHICLE_MOVEMENT:
					TurnInfo.Army.Deployments.ForEach(i => i.EnterUnits(true));
					TurnInfo.Army.Deployments.ForEach(i => i.AutomateMovement(true));
					return !TurnInfo.Army.Units.Any(i => i.CanMove(true, false) == OrderInvalidReason.NONE);
				case TurnComponent.CLOSE_ASSAULT:
					return !TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.CLOSE_ASSAULT) == OrderInvalidReason.NONE);
				case TurnComponent.NON_VEHICLE_MOVEMENT:
					TurnInfo.Army.Deployments.ForEach(i => i.EnterUnits(false));
					TurnInfo.Army.Deployments.ForEach(i => i.AutomateMovement(false));
					return !TurnInfo.Army.Units.Any(i => i.CanMove(false, false) == OrderInvalidReason.NONE);

				case TurnComponent.WAIT:
					return !Match.Scenario.FogOfWar;
				case TurnComponent.RESET:
					return true;
			}
			return false;
		}

		public void BufferOrder(Order Order, TurnInfo TurnInfo)
		{
			_MultiTurnAutomater.BufferOrder(Order, TurnInfo);
		}

		void DoMinefieldAttacks(Match Match, Army Army)
		{
			foreach (Unit u in Army.Units)
			{
				if (u.Position == null) continue;

				var mine = u.Position.Units.FirstOrDefault(i => i.Configuration.UnitClass == UnitClass.MINEFIELD);
				if (mine != null && mine.Position != null)
				{
					AttackOrder order = new MinefieldAttackOrder(mine.Army, mine.Position);
					var attack = new MinefieldSingleAttackOrder(mine);
					order.AddAttacker(attack);
					Match.ExecuteOrder(order);
				}
			}
		}
	}
}