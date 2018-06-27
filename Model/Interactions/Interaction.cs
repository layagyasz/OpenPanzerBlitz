namespace PanzerBlitz
{
	public interface Interaction
	{
		bool IsWork { get; }
		Unit Master { get; }
		OrderInvalidReason Validate();
		bool Apply(Unit Unit);
		bool Cancel();
	}
}
