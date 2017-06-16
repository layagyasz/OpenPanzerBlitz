using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Match
	{
		public readonly Scenario Scenario;
		public readonly List<Army> Armies;

		private IEnumerator<Tuple<Army, TurnComponent>> _TurnOrder;

		private Tuple<Army, TurnComponent> _CurrentPhase;
		private bool _Started;

		private Random _Random = new Random();

		public Match(Scenario Scenario)
		{
			this.Scenario = Scenario;
			Armies = Scenario.ArmyConfigurations.Select(i => new Army(i)).ToList();
			_TurnOrder = Enumerable.Repeat(
				StandardTurnOrder(Armies), Scenario.Turns).SelectMany(i => i).GetEnumerator();
		}

		public void Start()
		{
			NextPhase();
		}

		public Tuple<Army, TurnComponent> GetCurrentPhase()
		{
			return _CurrentPhase;
		}

		public bool NextPhase()
		{
			if (!_Started)
			{
				Army deploying = DeploymentPhase();
				if (_Started)
				{
					foreach (Army a in Armies) a.EndPhase(TurnComponent.DEPLOYMENT);
					_TurnOrder.Current.Item1.StartPhase(_TurnOrder.Current.Item2);
					_CurrentPhase = _TurnOrder.Current;
				}
				else
					_CurrentPhase = new Tuple<Army, TurnComponent>(deploying, TurnComponent.DEPLOYMENT);
				return _Started;
			}
			else
			{
				_TurnOrder.Current.Item1.EndPhase(_TurnOrder.Current.Item2);
				_TurnOrder.MoveNext();
				_TurnOrder.Current.Item1.StartPhase(_TurnOrder.Current.Item2);
				_CurrentPhase = _TurnOrder.Current;
				return true;
			}
		}

		public bool ExecuteOrder(Order Order)
		{
			if (Order is AttackOrder)
			{
				if (!ValidateAttackOrder((AttackOrder)Order)) return false;
			}
			return Order.Execute(_Random);
		}

		private bool ValidateAttackOrder(AttackOrder Order)
		{
			if (Order.AttackingArmy != _CurrentPhase.Item1)
			{
				if (Order.AttackMethod == AttackMethod.OVERRUN
					&& _CurrentPhase.Item2 != TurnComponent.ATTACK_MOVEMENT)
					return false;
				if (Order.AttackMethod == AttackMethod.NORMAL_FIRE
					&& _CurrentPhase.Item2 != TurnComponent.ATTACK)
					return false;
				if (Order.AttackMethod == AttackMethod.CLOSE_ASSAULT
					&& _CurrentPhase.Item2 != TurnComponent.CLOSE_ASSAULT)
					return false;
			}
			return true;
		}

		private Army DeploymentPhase()
		{
			IEnumerable<Army> undeployed = Armies.Where(i => !i.IsDeployed());
			foreach (Army a in undeployed)
			{
				a.Deploy();
				if (!Scenario.SimultaneousDeployment) return a;
			}
			return null;
		}

		private static IEnumerable<Tuple<Army, TurnComponent>> StandardTurnOrder(IEnumerable<Army> Armies)
		{
			return Armies.SelectMany(i => TurnComponents().Select(j => new Tuple<Army, TurnComponent>(i, j)));
		}

		private static IEnumerable<TurnComponent> TurnComponents()
		{
			yield return TurnComponent.ATTACK;
			yield return TurnComponent.ATTACK_MOVEMENT;
			yield return TurnComponent.MOVEMENT;
			yield return TurnComponent.CLOSE_ASSAULT;
		}
	}
}
