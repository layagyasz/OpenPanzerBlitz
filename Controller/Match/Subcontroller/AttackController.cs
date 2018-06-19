namespace PanzerBlitz
{
	public class AttackController : BaseAttackController
	{
		public AttackController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void HandleTileLeftClick(Tile Tile) { }

		public override void HandleTileRightClick(Tile Tile) { }

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.CanAttack(AttackMethod.DIRECT_FIRE) == OrderInvalidReason.NONE)
				SelectUnit(Unit, AttackMethod.DIRECT_FIRE);
			else if (Unit.Army != _Controller.CurrentTurn.Army)
			{
				if (_Controller.SelectedUnit != null)
				{
					AddAttack(
						Unit.Position,
						new DirectFireSingleAttackOrder(
							_Controller.SelectedUnit, Unit, _Controller.UseSecondaryWeapon()));
				}
			}
		}
	}
}
