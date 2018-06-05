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
			if (_AttackBuilder == null
				|| _AttackBuilder.TargetTile != Tile
				|| !_AttackBuilder.IsCompatible(NewAttack))
				attack = NewAttack.GenerateNewAttackOrder();
			else attack = _AttackBuilder;
			var r = attack.AddAttacker(NewAttack);
			if (r == OrderInvalidReason.NONE)
			{
				if (attack != _AttackBuilder)
				{
					_Controller.Clear();

					_AttackBuilder = attack;
					var attackPane = new AttackPane(_AttackBuilder);
					attackPane.OnAttackTargetChanged += ChangeAttackTarget;
					attackPane.OnExecute += ExecuteAttack;

					_Controller.SetPane(attackPane);
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
