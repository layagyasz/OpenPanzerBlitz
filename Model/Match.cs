using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Match
	{
		public EventHandler<ExecuteOrderEventArgs> OnExecuteOrder;

		public readonly Scenario Scenario;
		public readonly Map Map;
		public readonly List<Army> Armies;
		public readonly List<Order> ExecutedOrders = new List<Order>();

		private IEnumerator<Tuple<Army, TurnComponent>> _TurnOrder;

		private Random _Random = new Random();

		public Tuple<Army, TurnComponent> CurrentPhase
		{
			get
			{
				return _TurnOrder.Current;
			}
		}

		public Match(Scenario Scenario)
		{
			this.Scenario = Scenario;
			this.Map = new Map(Scenario.MapConfiguration);

			Armies = Scenario.ArmyConfigurations.Select(i => new Army(i)).ToList();
			_TurnOrder = Scenario.DeploymentOrder.Select(
				i => new Tuple<Army, TurnComponent>(
					Armies.Find(j => j.ArmyConfiguration == i), TurnComponent.DEPLOYMENT)).Concat(
						Enumerable.Repeat(StandardTurnOrder(Armies), Scenario.Turns)
						.SelectMany(i => i)).GetEnumerator();
		}

		public void Start()
		{
			NextPhase();
		}

		private void NextPhase()
		{
			if (_TurnOrder.Current != null) _TurnOrder.Current.Item1.EndPhase(_TurnOrder.Current.Item2);
			_TurnOrder.MoveNext();
			while (Automate()) _TurnOrder.MoveNext();
			_TurnOrder.Current.Item1.StartPhase(_TurnOrder.Current.Item2);
		}

		private bool Automate()
		{
			return _TurnOrder.Current.Item1.AutomatePhase(this, _TurnOrder.Current.Item2);
		}

		public bool ExecuteOrder(Order Order)
		{
			if (Order is AttackOrder)
			{
				if (!ValidateAttackOrder((AttackOrder)Order)) return false;
			}
			if (Order is DeployOrder)
			{
				if (!ValidateDeployOrder((DeployOrder)Order)) return false;
			}
			if (Order is NextPhaseOrder)
			{
				if (!ValidateNextPhaseOrder()) return false;
				else NextPhase();
			}
			bool executed = Order.Execute(_Random);
			if (executed)
			{
				if (OnExecuteOrder != null) OnExecuteOrder(this, new ExecuteOrderEventArgs(Order));
				ExecutedOrders.Add(Order);
			}
			return executed;
		}

		private bool ValidateNextPhaseOrder()
		{
			if (CurrentPhase.Item2 == TurnComponent.DEPLOYMENT) return CurrentPhase.Item1.IsDeploymentConfigured();
			return true;

		}

		private bool ValidateAttackOrder(AttackOrder Order)
		{
			if (Order.AttackingArmy != CurrentPhase.Item1)
				return false;
			if (Order.AttackMethod == AttackMethod.OVERRUN
				&& CurrentPhase.Item2 != TurnComponent.ATTACK_MOVEMENT)
				return false;
			if (Order.AttackMethod == AttackMethod.NORMAL_FIRE
				&& CurrentPhase.Item2 != TurnComponent.ATTACK)
				return false;
			if (Order.AttackMethod == AttackMethod.CLOSE_ASSAULT
				&& CurrentPhase.Item2 != TurnComponent.CLOSE_ASSAULT)
				return false;
			return true;
		}

		private bool ValidateDeployOrder(DeployOrder Order)
		{
			return (Order.Unit.Army == CurrentPhase.Item1 || CurrentPhase.Item1 == null)
				&& CurrentPhase.Item2 == TurnComponent.DEPLOYMENT;
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
