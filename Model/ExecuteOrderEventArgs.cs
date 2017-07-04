using System;
namespace PanzerBlitz
{
	public class ExecuteOrderEventArgs
	{
		public readonly Order Order;

		public ExecuteOrderEventArgs(Order Order)
		{
			this.Order = Order;
		}
	}
}
