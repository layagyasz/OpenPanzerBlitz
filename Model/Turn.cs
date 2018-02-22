namespace PanzerBlitz
{
	public struct Turn
	{
		public readonly byte TurnNumber;
		public readonly TurnInfo TurnInfo;

		public Turn(byte TurnNumber, TurnInfo TurnInfo)
		{
			this.TurnNumber = TurnNumber;
			this.TurnInfo = TurnInfo;
		}

		public override string ToString()
		{
			return string.Format("[Turn: TurnNumber={0}, TurnInfo={1}]", TurnNumber, TurnInfo);
		}
	}
}
