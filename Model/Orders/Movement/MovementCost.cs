namespace PanzerBlitz
{
	public struct MovementCost
	{
		public readonly OrderInvalidReason UnableReason;
		public readonly float Cost;

		public MovementCost(float Cost)
		{
			UnableReason = OrderInvalidReason.NONE;
			this.Cost = Cost;
		}

		public MovementCost(OrderInvalidReason UnableReason)
		{
			this.UnableReason = UnableReason;
			Cost = float.MaxValue;
		}

		public bool IsSet()
		{
			return UnableReason != OrderInvalidReason.NONE || Cost > 0;
		}

		public static MovementCost Min(MovementCost c1, MovementCost c2)
		{
			if (c1.UnableReason != OrderInvalidReason.NONE) return c2;
			if (c2.UnableReason != OrderInvalidReason.NONE) return c1;
			return c1.Cost > c2.Cost ? c2 : c1;
		}

		public static MovementCost Max(MovementCost c1, MovementCost c2)
		{
			if (c1.UnableReason != OrderInvalidReason.NONE) return c1;
			if (c2.UnableReason != OrderInvalidReason.NONE) return c2;
			return c1.Cost > c2.Cost ? c1 : c2;		}

		public static MovementCost operator +(MovementCost c1, MovementCost c2)
		{
			if (c1.UnableReason != OrderInvalidReason.NONE) return c1;
			if (c2.UnableReason != OrderInvalidReason.NONE) return c2;
			return new MovementCost(c1.Cost + c2.Cost);
		}

		public static MovementCost operator +(float m, MovementCost c)
		{
			if (c.UnableReason == OrderInvalidReason.NONE) return new MovementCost(m + c.Cost);
			return c;
		}

		public static MovementCost operator *(float m, MovementCost c)
		{
			if (c.UnableReason == OrderInvalidReason.NONE) return new MovementCost(m * c.Cost);
			return c;
		}
	}
}
