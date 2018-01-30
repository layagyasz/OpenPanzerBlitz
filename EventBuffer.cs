using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class EventBuffer<T> where T : EventArgs
	{
		Queue<Tuple<Action<object, T>, Tuple<object, T>>> _Invocations =
			new Queue<Tuple<Action<object, T>, Tuple<object, T>>>();

		public void QueueEvent(Action<object, T> Handler, object Sender, T E)
		{
			lock (_Invocations)
			{
				_Invocations.Enqueue(
					new Tuple<Action<object, T>, Tuple<object, T>>(Handler, new Tuple<object, T>(Sender, E)));
			}
		}

		public Action<object, T> Hook(Action<object, T> Handler)
		{
			return (Sender, E) => QueueEvent(Handler, Sender, E);
		}

		public void DispatchEvents()
		{
			lock (_Invocations)
			{
				foreach (var invocation in _Invocations)
					invocation.Item1(invocation.Item2.Item1, invocation.Item2.Item2);
				_Invocations.Clear();
			}
		}
	}
}
