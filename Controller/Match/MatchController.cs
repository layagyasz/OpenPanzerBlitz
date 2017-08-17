using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class MatchController
	{
		Match _Match;
		Dictionary<Army, MatchPlayerController> _PlayerControllers;
		bool _Automated;

		public MatchController(
			Match Match, Dictionary<Army, MatchPlayerController> PlayerControllers, bool Automated = false)
		{
			_Match = Match;
			_PlayerControllers = PlayerControllers;
			_Automated = Automated;

			if (_Automated)
			{
				_Match.Armies.ForEach(
					i => i.Deployments.ForEach(
						j => j.Units.ForEach(
							k => k.OnMove += (sender, e) => j.AutomateMovement(_Match, k.Configuration.IsVehicle))));
			}

			Match.OnStartPhase += HandleTurn;
		}

		public void DoTurn(TurnInfo TurnInfo)
		{
			if (_Automated && AutomateTurn(TurnInfo)) _Match.ExecuteOrder(new NextPhaseOrder());
			else if (TurnInfo.TurnComponent == TurnComponent.RESET) AutomateTurn(TurnInfo);
			else _PlayerControllers[TurnInfo.Army].DoTurn(TurnInfo);
		}

		void HandleTurn(object Sender, StartTurnComponentEventArgs E)
		{
			DoTurn(E.TurnInfo);
		}

		bool AutomateTurn(TurnInfo TurnInfo)
		{
			switch (TurnInfo.TurnComponent)
			{
				case TurnComponent.ATTACK:
					return !TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.NORMAL_FIRE) == NoSingleAttackReason.NONE);
				case TurnComponent.CLOSE_ASSAULT:
					return !TurnInfo.Army.Units.Any(
						i => i.CanAttack(AttackMethod.CLOSE_ASSAULT) == NoSingleAttackReason.NONE);
				case TurnComponent.DEPLOYMENT:
					return TurnInfo.Army.Deployments.All(i => i.AutomateDeployment(_Match));
				case TurnComponent.MINEFIELD_ATTACK:
					DoMinefieldAttacks(TurnInfo.Army);
					return true;
				case TurnComponent.NON_VEHICLE_MOVEMENT:
					TurnInfo.Army.Deployments.ForEach(i => i.AutomateMovement(_Match, false));
					return !TurnInfo.Army.Units.Any(i => i.CanMove(false, false) == NoMoveReason.NONE);
				case TurnComponent.RESET:
					foreach (Unit u in TurnInfo.Army.Units) u.Reset();
					return true;
				case TurnComponent.VEHICLE_COMBAT_MOVEMENT:
					return !TurnInfo.Army.Units.Any(i => i.CanMove(true, true) == NoMoveReason.NONE);
				case TurnComponent.VEHICLE_MOVEMENT:
					TurnInfo.Army.Deployments.ForEach(i => i.AutomateMovement(_Match, true));
					return !TurnInfo.Army.Units.Any(i => i.CanMove(true, false) == NoMoveReason.NONE);
			}
			return false;
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
						_Match.ExecuteOrder(order);
					}
				}
			}
		}
	}
}