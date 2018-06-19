namespace PanzerBlitz
{
	public class AircraftController : BaseAttackController
	{
		public AircraftController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_Controller.SelectedUnit != null)
			{
				var order = new MovementOrder(_Controller.SelectedUnit, Tile, false);
				if (_Controller.ExecuteOrderAndAlert(order)) SelectUnit(_Controller.SelectedUnit, AttackMethod.AIR);
			}
		}

		public override void HandleTileRightClick(Tile Tile) { }

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.Configuration.IsAircraft())
				SelectUnit(Unit, AttackMethod.AIR);
			else if (Unit.Army != _Controller.CurrentTurn.Army)
			{
				if (_Controller.SelectedUnit != null)
				{
					AddAttack(
						Unit.Position,
						new AirSingleAttackOrder(
							_Controller.SelectedUnit, Unit, _Controller.UseSecondaryWeapon()));
				}
			}
		}
	}
}
