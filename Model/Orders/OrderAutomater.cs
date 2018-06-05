namespace PanzerBlitz
{
	public interface OrderAutomater
	{
		void Hook(EventRelay Relay);
		bool AutomateTurn(Match Match, TurnInfo TurnInfo);
		void BufferOrder(Order Order, TurnInfo TurnInfo);
	}
}
