namespace PanzerBlitz
{
	public struct UnitVisibility
	{
		public bool Visible { get; set; }
		public Tile LastSeen { get; set; }

		public UnitVisibility(bool Visible, Tile LastSeen)
		{
			this.Visible = Visible;
			this.LastSeen = LastSeen;
		}

		public override string ToString()
		{
			return string.Format("[UnitVisibility: Visible={0}, LastSeen={1}]", Visible, LastSeen);
		}
	}
}
