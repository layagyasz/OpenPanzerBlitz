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
		public readonly IdGenerator IdGenerator = new IdGenerator();

		IEnumerator<TurnInfo> _TurnOrder;

		OrderAutomator _OrderAutomater;
		List<Order> _OrderBuffer = new List<Order>();
		Random _Random = new Random();

		public TurnInfo CurrentPhase
		{
			get
			{
				return _TurnOrder.Current;
			}
		}

		public Match(Scenario Scenario, bool AutomateTurns = true)
		{
			this.Scenario = Scenario;
			Map = Scenario.MapConfiguration.GenerateMap(Scenario.Environment.TileRuleSet, IdGenerator);

			Armies = Scenario.TurnOrder.Select(i => new Army(this, i, IdGenerator)).ToList();
			_TurnOrder = Scenario.DeploymentOrder.Select(
				i => new TurnInfo(
					Armies.Find(j => j.Configuration == i), TurnComponent.DEPLOYMENT))
								 .Concat(Armies.Select(i => new TurnInfo(i, TurnComponent.RESET)))
								 .Concat(Enumerable.Repeat(StandardTurnOrder(Armies), Scenario.Turns)
									.SelectMany(i => i)).GetEnumerator();
			foreach (Unit u in Armies.SelectMany(i => i.Units))
			{
				u.OnMove += UpdateUnitVisibilityFromMove;
				u.OnFire += UpdateUnitVisibilityFromFire;
			}
			if (AutomateTurns) _OrderAutomater = new OrderAutomator(this);
		}

		public Dictionary<Army, ObjectiveSuccessLevel> GetArmyObjectiveSuccessLevels()
		{
			Dictionary<Army, ObjectiveSuccessLevel> r = new Dictionary<Army, ObjectiveSuccessLevel>();
			foreach (Army a in Armies) r.Add(a, a.GetObjectiveSuccessLevel(this));
			return r;
		}

		public IEnumerable<GameObject> GetGameObjects()
		{
			return Map.TilesEnumerable.Concat(Armies.SelectMany(i => i.GetGameObjects()));
		}

		public void Start()
		{
			lock (_OrderBuffer)
			{
				NextPhase();
				DoBufferedOrders();
			}
		}

		void NextPhase()
		{
			if (!AdvancePhaseIterator()) return;
			if (_OrderAutomater != null && _OrderAutomater.AutomateTurn(_TurnOrder.Current))
				ExecuteOrder(new NextPhaseOrder(_TurnOrder.Current.Army));
			else if (OnStartPhase != null)
				OnStartPhase(this, new StartTurnComponentEventArgs(_TurnOrder.Current));
		}

		bool AdvancePhaseIterator()
		{
			if (_TurnOrder.MoveNext()) return true;

			if (OnMatchEnded != null) OnMatchEnded(this, EventArgs.Empty);
			return false;
		}

		public bool ValidateOrder(Order Order)
		{
			if (CurrentPhase == null) return false;
			if (Order.Army != CurrentPhase.Army) return false;
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
			else if (Order is ReconOrder)
			{
				if (!ValidateReconOrder((ReconOrder)Order)) return false;
			}
			else if (Order is EvacuateOrder)
			{
				if (!ValidateEvacuateOrder((EvacuateOrder)Order)) return false;
			}
			else if (Order is DeployOrder)
			{
				if (!ValidateDeployOrder((DeployOrder)Order)) return false;
			}
			else if (Order is ResetOrder)
			{
				if (!ValidateResetOrder((ResetOrder)Order)) return false;
			}
			else if (Order is NextPhaseOrder)
			{
				if (!ValidateNextPhaseOrder()) return false;
			}
			return true;
		}

		public bool BufferOrder(Order Order)
		{
			lock (_OrderBuffer)
			{
				_OrderBuffer.Add(Order);
			}
			return ValidateOrder(Order);
		}

		public void DoBufferedOrders()
		{
			lock (_OrderBuffer)
			{
				foreach (Order o in _OrderBuffer) ExecuteOrder(o);
				_OrderBuffer.Clear();
			}
		}

		public bool ExecuteOrder(Order Order)
		{
			if (!ValidateOrder(Order)) return false;

			if (Order is NextPhaseOrder)
			{
				if (OnExecuteOrder != null) OnExecuteOrder(this, new ExecuteOrderEventArgs(Order));
				ExecutedOrders.Add(Order);
				NextPhase();
				return true;
			}
			OrderStatus executed = Order.Execute(_Random);
			if (executed == OrderStatus.IN_PROGRESS && _OrderAutomater != null)
				_OrderAutomater.BufferOrder(Order, _TurnOrder.Current);
			if (executed != OrderStatus.ILLEGAL)
			{
				if (OnExecuteOrder != null) OnExecuteOrder(this, new ExecuteOrderEventArgs(Order));
				ExecutedOrders.Add(Order);
			}
			return executed != OrderStatus.ILLEGAL;
		}

		bool ValidateNextPhaseOrder()
		{
			if (CurrentPhase.TurnComponent == TurnComponent.DEPLOYMENT)
				return CurrentPhase.Army.IsDeploymentConfigured();
			if (CurrentPhase.TurnComponent == TurnComponent.VEHICLE_MOVEMENT)
				return !CurrentPhase.Army.MustMove(true);
			if (CurrentPhase.TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT)
				return !CurrentPhase.Army.MustMove(false);
			return true;
		}

		bool ValidateAttackOrder(AttackOrder Order)
		{
			if (Order.AttackMethod == AttackMethod.OVERRUN
				&& CurrentPhase.TurnComponent != TurnComponent.VEHICLE_COMBAT_MOVEMENT)
				return false;
			if (Order.AttackMethod == AttackMethod.NORMAL_FIRE
				&& CurrentPhase.TurnComponent != TurnComponent.ATTACK)
				return false;
			if (Order.AttackMethod == AttackMethod.CLOSE_ASSAULT
				&& CurrentPhase.TurnComponent != TurnComponent.CLOSE_ASSAULT)
				return false;
			return true;
		}

		bool ValidateUnloadOrder(UnloadOrder Order)
		{
			if (!Order.UseMovement) return CurrentPhase.TurnComponent == TurnComponent.DEPLOYMENT;
			if (Order.Carrier.Configuration.IsVehicle)
				return CurrentPhase.TurnComponent == TurnComponent.VEHICLE_MOVEMENT;
			return CurrentPhase.TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		bool ValidateLoadOrder(LoadOrder Order)
		{
			if (!Order.UseMovement) return CurrentPhase.TurnComponent == TurnComponent.DEPLOYMENT;
			if (Order.Carrier.Configuration.IsVehicle)
				return CurrentPhase.TurnComponent == TurnComponent.VEHICLE_MOVEMENT;
			return CurrentPhase.TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		bool ValidateMovementOrder(MovementOrder Order)
		{
			if (Order.Combat && !Order.Unit.Configuration.IsVehicle)
				return CurrentPhase.TurnComponent == TurnComponent.CLOSE_ASSAULT;
			if (Order.Unit.Configuration.IsVehicle) return CurrentPhase.TurnComponent == TurnComponent.VEHICLE_MOVEMENT;

			return CurrentPhase.TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		bool ValidateDeployOrder(DeployOrder Order)
		{
			if (Order is MovementDeployOrder)
			{
				if (CurrentPhase.TurnComponent == TurnComponent.VEHICLE_MOVEMENT)
					return ((MovementDeployOrder)Order).Unit.Configuration.IsVehicle;
				if (CurrentPhase.TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT)
					return !((MovementDeployOrder)Order).Unit.Configuration.IsVehicle;
				return false;
			}
			return (Order.Army == CurrentPhase.Army || CurrentPhase.Army == null)
				&& CurrentPhase.TurnComponent == TurnComponent.DEPLOYMENT;
		}

		bool ValidateReconOrder(ReconOrder Order)
		{
			if (Order.Unit.Configuration.IsVehicle) return CurrentPhase.TurnComponent == TurnComponent.VEHICLE_MOVEMENT;
			return CurrentPhase.TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		bool ValidateEvacuateOrder(EvacuateOrder Order)
		{
			if (Order.Unit.Configuration.IsVehicle) return CurrentPhase.TurnComponent == TurnComponent.VEHICLE_MOVEMENT;
			return CurrentPhase.TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		bool ValidateResetOrder(ResetOrder Order)
		{
			if (Order.CompleteReset) return CurrentPhase.TurnComponent == TurnComponent.RESET;
			return true;
		}

		void UpdateUnitVisibilityFromMove(object Sender, MovementEventArgs E)
		{
			Unit u = (Unit)Sender;
			foreach (Army a in Armies)
			{
				if (E.Path == null || E.Path.Count < 2) a.UpdateUnitVisibility(u, E.Tile);
				else a.UpdateUnitVisibility(u, E.Path[E.Path.Count - 2], E.Path.Destination);
			}
		}

		void UpdateUnitVisibilityFromFire(object Sender, EventArgs E)
		{
			Unit u = (Unit)Sender;
			foreach (Army a in Armies)
			{
				a.SetUnitVisibility(u, true);
			}
		}

		static IEnumerable<TurnInfo> StandardTurnOrder(IEnumerable<Army> Armies)
		{
			return Armies.SelectMany(i => TurnComponents().Select(j => new TurnInfo(i, j)));
		}

		static IEnumerable<TurnComponent> TurnComponents()
		{
			yield return TurnComponent.MINEFIELD_ATTACK;
			yield return TurnComponent.ATTACK;
			yield return TurnComponent.VEHICLE_COMBAT_MOVEMENT;
			yield return TurnComponent.VEHICLE_MOVEMENT;
			yield return TurnComponent.CLOSE_ASSAULT;
			yield return TurnComponent.NON_VEHICLE_MOVEMENT;
			yield return TurnComponent.RESET;
		}

		public override string ToString()
		{
			return string.Format(
				"[Match: CurrentPhase={0}, Scenario={1}]", CurrentPhase, Scenario.Name);
		}
	}
}
