namespace PanzerBlitz
{
	public interface OrderAutomater
	{
		void Hook(EventRelay Relay);
		bool AutomateTurn(Match Match, Turn Turn);
		void BufferOrder(Order Order, TurnInfo TurnInfo);
	}
}
