using SFML.Window;

namespace PanzerBlitz
{
	public abstract class BaseController : Subcontroller
	{
		protected HumanMatchPlayerController _Controller;

		protected BaseController(HumanMatchPlayerController Controller)
		{
			_Controller = Controller;
		}

		public virtual void Begin() { }

		public virtual bool Finish()
		{
			return true;
		}

		public virtual void End() { }

		public virtual bool CanLoad()
		{
			return false;
		}

		public virtual bool CanUnload()
		{
			return false;
		}

		public virtual bool CanDismount()
		{
			return false;
		}

		public virtual bool CanFortify()
		{
			return false;
		}

		public virtual bool CanAbandon()
		{
			return false;
		}

		public virtual bool CanMount()
		{
			return false;
		}

		public virtual bool CanEvacuate()
		{
			return false;
		}

		public virtual bool CanRecon()
		{
			return false;
		}

		public virtual bool CanClearMinefield()
		{
			return false;
		}

		public virtual bool CanEmplace()
		{
			return false;
		}

		public abstract void HandleTileLeftClick(Tile Tile);
		public abstract void HandleTileRightClick(Tile Tile);
		public abstract void HandleUnitLeftClick(Unit Unit);
		public abstract void HandleUnitRightClick(Unit Unit);
		public abstract void HandleKeyPress(Keyboard.Key Key);

		public virtual void HandleUnitShiftLeftClick(Unit Unit)
		{
			return;
		}

		public void HandleUnitShiftRightClick(Unit Unit)
		{
			var pane = new UnitInfoPane(Unit, _Controller.UnitConfigurationRenderer);
			pane.OnClose += (sender, e) => _Controller.Clear();
			_Controller.SetPane(pane);
		}
	}
}
