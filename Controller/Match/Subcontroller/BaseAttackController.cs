using System;

using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public abstract class BaseAttackController : BaseController
	{
		AttackOrder _AttackBuilder;

		protected BaseAttackController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void HandleUnitRightClick(Unit Unit)
		{
			if (_AttackBuilder != null) _AttackBuilder.RemoveAttacker(Unit);
		}

		public override void HandleKeyPress(Keyboard.Key Key) { }

		protected void AddAttack(Tile Tile, SingleAttackOrder NewAttack)
		{
			AttackOrder attack = null;
			if (_AttackBuilder == null || _AttackBuilder.TargetTile != Tile)
				attack = NewAttack.GenerateNewAttackOrder();
			else attack = _AttackBuilder;
			var r = attack.AddAttacker(NewAttack);
			if (r == OrderInvalidReason.NONE)
			{
				if (attack != _AttackBuilder)
				{
					_Controller.Clear();

					_AttackBuilder = attack;
					AttackPane attackPane = new AttackPane(_AttackBuilder);
					_Controller.AddPane(attackPane);

					attackPane.OnAttackTargetChanged += ChangeAttackTarget;
					attackPane.OnExecute += ExecuteAttack;

					_Controller.AddPane(attackPane);
				}
			}
			else _Controller.Alert(r);
		}

		void ExecuteAttack(object sender, EventArgs e)
		{
			if (_Controller.ExecuteOrderAndAlert(_AttackBuilder))
			{
				_Controller.Clear();
				_AttackBuilder = null;
			}
		}

		void ChangeAttackTarget(object Sender, ValuedEventArgs<AttackTarget> E)
		{
			_AttackBuilder.SetAttackTarget(E.Value);
		}
	}
}
