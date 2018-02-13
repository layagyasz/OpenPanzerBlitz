using System;

using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public abstract class BaseAttackController : BaseController
	{
		AttackOrder _AttackBuilder;
		AttackPane _AttackPane;

		public BaseAttackController(HumanMatchPlayerController Controller)
			: base(Controller)
		{
		}

		public override void Clear()
		{
			base.Clear();
			if (_AttackBuilder != null)
			{
				_Controller.RemovePane(_AttackPane);
				_AttackBuilder = null;
			}
		}

		public override void End()
		{
			base.End();
			Clear();
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
			if (_AttackBuilder != null) _AttackBuilder.RemoveAttacker(Unit);
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
		}

		protected void AddAttack(Tile Tile, SingleAttackOrder NewAttack)
		{
			AttackOrder attack = null;
			if (_AttackBuilder == null || _AttackBuilder.TargetTile != Tile)
				attack = NewAttack.GenerateNewAttackOrder();
			else attack = _AttackBuilder;
			OrderInvalidReason r = attack.AddAttacker(NewAttack);
			if (r == OrderInvalidReason.NONE)
			{
				if (attack != _AttackBuilder)
				{
					Clear();

					_AttackBuilder = attack;
					_AttackPane = new AttackPane(_AttackBuilder);
					_Controller.AddPane(_AttackPane);

					_AttackPane.OnAttackTargetChanged += ChangeAttackTarget;
					_AttackPane.OnExecute += ExecuteAttack;
				}
			}
			else _Controller.Alert(r);
		}

		void ExecuteAttack(object sender, EventArgs e)
		{
			if (_Controller.ExecuteOrderAndAlert(_AttackBuilder))
			{
				_Controller.RemovePane(_AttackPane);
				_AttackBuilder = null;
			}
		}

		void ChangeAttackTarget(object Sender, ValuedEventArgs<AttackTarget> E)
		{
			_AttackBuilder.SetAttackTarget(E.Value);
		}
	}
}
