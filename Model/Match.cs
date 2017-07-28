using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Match
	{
		public EventHandler<ExecuteOrderEventArgs> OnExecuteOrder;
		public EventHandler<StartTurnComponentEventArgs> OnStartPhase;
		public EventHandler<EventArgs> OnMatchEnded;

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
					Armies.Find(j => j.Configuration == i), TurnComponent.DEPLOYMENT))
								 .Concat(Armies.Select(i => new Tuple<Army, TurnComponent>(i, TurnComponent.RESET)))
								 .Concat(Enumerable.Repeat(StandardTurnOrder(Armies), Scenario.Turns)
									.SelectMany(i => i)).GetEnumerator();

			Armies.ForEach(
				i => i.Deployments.ForEach(
					j => j.Units.ForEach(
						k => k.OnMove += (sender, e) => j.AutomateMovement(this, k.Configuration.IsVehicle))));
		}

		public Dictionary<Army, ObjectiveSuccessLevel> GetArmyObjectiveSuccessLevels()
		{
			Dictionary<Army, ObjectiveSuccessLevel> r = new Dictionary<Army, ObjectiveSuccessLevel>();
			foreach (Army a in Armies) r.Add(a, a.GetObjectiveSuccessLevel(this));
			return r;
		}

		public void Start()
		{
			NextPhase();
		}

		void NextPhase()
		{
			if (!AdvancePhaseIterator()) return;
			while (Automate())
			{
				if (!AdvancePhaseIterator()) return;
			}
			if (OnStartPhase != null)
				OnStartPhase(this, new StartTurnComponentEventArgs(_TurnOrder.Current.Item1, _TurnOrder.Current.Item2));
		}

		bool Automate()
		{
			return _TurnOrder.Current.Item1.AutomatePhase(this, _TurnOrder.Current.Item2, _Random);
		}

		bool AdvancePhaseIterator()
		{
			if (_TurnOrder.MoveNext()) return true;

			if (OnMatchEnded != null) OnMatchEnded(this, EventArgs.Empty);
			return false;
		}

		public bool ValidateOrder(Order Order)
		{
			if (Order is AttackOrder)
			{
				if (!ValidateAttackOrder((AttackOrder)Order)) return false;
			}
			else if (Order is MovementOrder)
			{
				if (!ValidateMovementOrder((MovementOrder)Order)) return false;
			}
			else if (Order is LoadOrder)
			{
				if (!ValidateLoadOrder((LoadOrder)Order)) return false;
			}
			else if (Order is UnloadOrder)
			{
				if (!ValidateUnloadOrder((UnloadOrder)Order)) return false;
			}
			else if (Order is DeployOrder)
			{
				if (!ValidateDeployOrder((DeployOrder)Order)) return false;
			}
			else if (Order is NextPhaseOrder)
			{
				if (!ValidateNextPhaseOrder()) return false;
			}
			return true;
		}

		public bool ExecuteOrder(Order Order)
		{
			if (!ValidateOrder(Order)) return false;
			if (Order is NextPhaseOrder)
			{
				NextPhase();
				return true;
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
			if (CurrentPhase.Item2 == TurnComponent.VEHICLE_MOVEMENT) return !CurrentPhase.Item1.MustMove(true);
			if (CurrentPhase.Item2 == TurnComponent.NON_VEHICLE_MOVEMENT) return !CurrentPhase.Item1.MustMove(false);
			return true;
		}

		private bool ValidateAttackOrder(AttackOrder Order)
		{
			if (Order.AttackingArmy != CurrentPhase.Item1)
				return false;
			if (Order.AttackMethod == AttackMethod.OVERRUN
				&& CurrentPhase.Item2 != TurnComponent.VEHICLE_COMBAT_MOVEMENT)
				return false;
			if (Order.AttackMethod == AttackMethod.NORMAL_FIRE
				&& CurrentPhase.Item2 != TurnComponent.ATTACK)
				return false;
			if (Order.AttackMethod == AttackMethod.CLOSE_ASSAULT
				&& CurrentPhase.Item2 != TurnComponent.CLOSE_ASSAULT)
				return false;
			return true;
		}

		private bool ValidateUnloadOrder(UnloadOrder Order)
		{
			if (!Order.UseMovement) return CurrentPhase.Item2 == TurnComponent.DEPLOYMENT;
			if (Order.Carrier.Configuration.IsVehicle) return CurrentPhase.Item2 == TurnComponent.VEHICLE_MOVEMENT;
			return CurrentPhase.Item2 == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		private bool ValidateLoadOrder(LoadOrder Order)
		{
			if (!Order.UseMovement) return CurrentPhase.Item2 == TurnComponent.DEPLOYMENT;
			if (Order.Carrier.Configuration.IsVehicle) return CurrentPhase.Item2 == TurnComponent.VEHICLE_MOVEMENT;
			return CurrentPhase.Item2 == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		private bool ValidateMovementOrder(MovementOrder Order)
		{
			if (Order.Combat && !Order.Unit.Configuration.IsVehicle)
				return CurrentPhase.Item2 == TurnComponent.CLOSE_ASSAULT;
			if (Order.Unit.Configuration.IsVehicle) return CurrentPhase.Item2 == TurnComponent.VEHICLE_MOVEMENT;

			return CurrentPhase.Item2 == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		private bool ValidateDeployOrder(DeployOrder Order)
		{
			if (Order is MovementDeployOrder)
			{
				if (CurrentPhase.Item2 == TurnComponent.VEHICLE_MOVEMENT)
					return ((MovementDeployOrder)Order).Unit.Configuration.IsVehicle;
				if (CurrentPhase.Item2 == TurnComponent.NON_VEHICLE_MOVEMENT)
					return !((MovementDeployOrder)Order).Unit.Configuration.IsVehicle;
				return false;
			}
			return (Order.Army == CurrentPhase.Item1 || CurrentPhase.Item1 == null)
				&& CurrentPhase.Item2 == TurnComponent.DEPLOYMENT;
		}

		private static IEnumerable<Tuple<Army, TurnComponent>> StandardTurnOrder(IEnumerable<Army> Armies)
		{
			return Armies.SelectMany(i => TurnComponents().Select(j => new Tuple<Army, TurnComponent>(i, j)));
		}

		private static IEnumerable<TurnComponent> TurnComponents()
		{
			yield return TurnComponent.MINEFIELD_ATTACK;
			yield return TurnComponent.ATTACK;
			yield return TurnComponent.VEHICLE_COMBAT_MOVEMENT;
			yield return TurnComponent.VEHICLE_MOVEMENT;
			yield return TurnComponent.CLOSE_ASSAULT;
			yield return TurnComponent.NON_VEHICLE_MOVEMENT;
			yield return TurnComponent.RESET;
		}
	}
}
