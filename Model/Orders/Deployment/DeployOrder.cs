using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public abstract class DeployOrder : Order
	{
		public Order CloneIfStateful()
		{
			return this;
		}

		public abstract void Serialize(SerializationOutputStream Stream);
		public abstract Army Army { get; }
		public abstract bool MatchesTurnComponent(TurnComponent TurnComponent);
		public abstract OrderInvalidReason Validate();
		public abstract OrderStatus Execute(Random Random);
	}
}
