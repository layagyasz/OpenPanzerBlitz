using System;
namespace PanzerBlitz
{
	public class NewUnitEventArgs : EventArgs
	{
		public readonly Unit Unit;

		public NewUnitEventArgs(Unit Unit)
		{
			this.Unit = Unit;
		}
	}
}
