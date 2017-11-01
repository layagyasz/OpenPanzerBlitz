using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface Order : Serializable
	{
		Army Army { get; }
		bool MatchesTurnComponent(TurnComponent TurnComponent);
		OrderInvalidReason Validate();
		OrderStatus Execute(Random Random);
	}
}
