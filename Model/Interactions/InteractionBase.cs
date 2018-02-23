namespace PanzerBlitz
{
	public abstract class InteractionBase : Interaction
	{
		public readonly Unit Agent;
		public readonly Unit Object;

		public Unit Master
		{
			get
			{
				return Agent;
			}
		}

		protected InteractionBase(Unit Agent, Unit Object)
		{
			this.Agent = Agent;
			this.Object = Object;
		}

		public abstract OrderInvalidReason Validate();
		public abstract bool Apply(Unit Unit);

		public bool Cancel()
		{
			Agent.CancelInteraction(this);
			Object.CancelInteraction(this);
			return true;
		}

		public override string ToString()
		{
			return string.Format("[ClearMinefieldInteraction: Agent={0}, Object={1}]", Agent, Object);
		}
	}
}
