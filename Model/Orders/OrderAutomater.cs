using System;
namespace PanzerBlitz
{
	public interface OrderAutomater
	{
		bool AutomateTurn(TurnInfo TurnInfo);
		void BufferOrder(Order Order, TurnInfo TurnInfo);
	}
}
