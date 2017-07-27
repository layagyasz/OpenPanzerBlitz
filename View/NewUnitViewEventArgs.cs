using System;
namespace PanzerBlitz
{
	public class NewUnitViewEventArgs : EventArgs
	{
		public readonly UnitView UnitView;

		public NewUnitViewEventArgs(UnitView UnitView)
		{
			this.UnitView = UnitView;
		}
	}
}
