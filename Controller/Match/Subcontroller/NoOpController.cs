using SFML.Window;

namespace PanzerBlitz
{
	public class NoOpController : BaseController
	{
		public NoOpController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override bool CanLoad()
		{
			return false;
		}

		public override bool CanUnload()
		{
			return false;
		}

		public override bool CanDismount()
		{
			return false;
		}

		public override bool CanMount()
		{
			return false;
		}

		public override bool CanEvacuate()
		{
			return false;
		}

		public override bool CanRecon()
		{
			return false;
		}

		public override bool CanClearMinefield()
		{
			return false;
		}

		public override bool CanEmplace()
		{
			return false;
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			return;
		}

		public override void HandleTileRightClick(Tile Tile)
		{
			return;
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			return;
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
			return;
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
			return;
		}
	}
}
