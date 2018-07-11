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

		public readonly EventRelay Relay = new EventRelay();
		public readonly Scenario Scenario;
		public readonly Map Map;
		public readonly List<Army> Armies;
		public readonly List<Order> ExecutedOrders = new List<Order>();
		public readonly IdGenerator IdGenerator = new IdGenerator();

		IEnumerator<Turn> _TurnOrder;

		OrderAutomater _OrderAutomater;
		List<Order> _OrderBuffer = new List<Order>();
		Random _Random = new Random();

		public Turn CurrentTurn
		{
			get
			{
				return _TurnOrder.Current;
			}
		}

		public Match(Scenario Scenario, OrderAutomater OrderAutomater)
		{
			this.Scenario = Scenario;
			Map = Scenario.MapConfiguration.GenerateMap(new Random(), Scenario.Environment, IdGenerator);

			Armies = Scenario.ArmyConfigurations.Select(
				i => new Army(
					this,
					new LazySightFinder(
						Scenario.FogOfWar ? (UnitTracker)new FogOfWarUnitTracker() : new OmniscientUnitTracker()),
					i,
					IdGenerator)).ToList();
			_TurnOrder = Scenario.TurnConfiguration.Materialize(new Random(), Armies).GetEnumerator();
			Relay.Hook(this);

			_OrderAutomater = OrderAutomater;
			if (_OrderAutomater != null) _OrderAutomater.Hook(Relay);
		}

		public Dictionary<Army, ObjectiveSuccessLevel> GetArmyObjectiveSuccessLevels()
		{
			var r = new Dictionary<Army, ObjectiveSuccessLevel>();
			foreach (Army a in Armies) r.Add(a, a.GetObjectiveSuccessLevel());
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
			if (_OrderAutomater != null && _OrderAutomater.AutomateTurn(this, _TurnOrder.Current.TurnInfo))
				ExecuteOrder(new NextPhaseOrder(_TurnOrder.Current.TurnInfo));
			else if (OnStartPhase != null)
				OnStartPhase(this, new StartTurnComponentEventArgs(_TurnOrder.Current));
		}

		bool AdvancePhaseIterator()
		{
			Turn lastTurn = _TurnOrder.Current;
			if (_TurnOrder.MoveNext())
			{
				// Check to see if any stop early victory conditions are met.
				if (_TurnOrder.Current.TurnNumber == lastTurn.TurnNumber
					|| Armies.Max(i => i.CheckObjectiveSuccessLevel()) == ObjectiveSuccessLevel.NONE)
					return true;
			}

			if (OnMatchEnded != null) OnMatchEnded(this, EventArgs.Empty);
			return false;
		}

		public OrderInvalidReason ValidateOrder(Order Order)
		{
			if (CurrentTurn.TurnInfo.Army == null) return OrderInvalidReason.ORDER_TURN_COMPONENT;
			return Order.Army == CurrentTurn.TurnInfo.Army
						&& Order.MatchesTurnComponent(CurrentTurn.TurnInfo.TurnComponent)
						? Order.Validate() : OrderInvalidReason.ORDER_TURN_COMPONENT;
		}

		public OrderInvalidReason BufferOrder(Order Order)
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

		public OrderInvalidReason ExecuteOrder(Order Order)
		{
			var r = ValidateOrder(Order);
			if (r != OrderInvalidReason.NONE) return r;

			ExecutedOrders.Add(Order.CloneIfStateful());
			if (OnExecuteOrder != null) OnExecuteOrder(this, new ExecuteOrderEventArgs(Order));

			if (Order is NextPhaseOrder)
			{
				NextPhase();
				Order.Execute(_Random);
				return OrderInvalidReason.NONE;
			}

			var executed = Order.Execute(_Random);
			if (executed == OrderStatus.IN_PROGRESS && _OrderAutomater != null)
			{
				_OrderAutomater.BufferOrder(Order, _TurnOrder.Current.TurnInfo);
			}
			if (executed == OrderStatus.ILLEGAL)
				throw new Exception(string.Format("Tried to execute illegal order. {0} {1}", Order, Order.Validate()));

			return OrderInvalidReason.NONE;
		}

		public override string ToString()
		{
			return string.Format(
				"[Match: CurrentPhase={0}, Scenario={1}]", CurrentTurn, Scenario.Name);
		}
	}
}
