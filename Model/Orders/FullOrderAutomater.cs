using System.Linq;

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
			unit.Deployment?.EnterUnits(unit.Army.Match.CurrentTurn, unit.Configuration.IsVehicle);
		}

		public bool AutomateTurn(Match Match, Turn Turn)
		{
			Match.ExecuteOrder(new ResetOrder(Turn.TurnInfo.Army, Turn.TurnInfo.TurnComponent == TurnComponent.RESET));

			_MultiTurnAutomater.AutomateTurn(Match, Turn);

			switch (Turn.TurnInfo.TurnComponent)
			{
				case TurnComponent.DEPLOYMENT:
					Turn.TurnInfo.Army.Deployments.ForEach(i => i.AutomateDeployment());
					return false;

				case TurnComponent.MINEFIELD_ATTACK:
					DoMinefieldAttacks(Match, Turn.TurnInfo.Army);
					return true;
				case TurnComponent.ARTILLERY:
					return !Turn.TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.INDIRECT_FIRE) == OrderInvalidReason.NONE);
				case TurnComponent.ATTACK:
					return !Turn.TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.DIRECT_FIRE) == OrderInvalidReason.NONE);

				case TurnComponent.AIRCRAFT:
					return !Turn.TurnInfo.Army.Units.Any(
						i => i.Configuration.IsAircraft() && i.Status == UnitStatus.ACTIVE);
				case TurnComponent.ANTI_AIRCRAFT:
					return !Turn.TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.ANTI_AIRCRAFT) == OrderInvalidReason.NONE)
						|| !Match.Armies.Where(i => i.Configuration.Team != Turn.TurnInfo.Army.Configuration.Team)
						.SelectMany(i => i.Units)
						.Any(i => i.Configuration.IsAircraft() && i.Position != null);

				case TurnComponent.VEHICLE_COMBAT_MOVEMENT:
					return !Turn.TurnInfo.Army.Units.Any(i => i.CanMove(true, true) == OrderInvalidReason.NONE
													&& i.CanAttack(AttackMethod.OVERRUN) == OrderInvalidReason.NONE);
				case TurnComponent.VEHICLE_MOVEMENT:
					Turn.TurnInfo.Army.Deployments.ForEach(i => i.EnterUnits(Turn, true));
					Turn.TurnInfo.Army.Deployments.ForEach(i => i.AutomateMovement(true));
					return !Turn.TurnInfo.Army.Units.Any(i => i.CanMove(true, false) == OrderInvalidReason.NONE);
				case TurnComponent.CLOSE_ASSAULT:
					return !Turn.TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.CLOSE_ASSAULT) == OrderInvalidReason.NONE);
				case TurnComponent.NON_VEHICLE_MOVEMENT:
					Turn.TurnInfo.Army.Deployments.ForEach(i => i.EnterUnits(Turn, false));
					Turn.TurnInfo.Army.Deployments.ForEach(i => i.AutomateMovement(false));
					return !Turn.TurnInfo.Army.Units.Any(i => i.CanMove(false, false) == OrderInvalidReason.NONE);

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
