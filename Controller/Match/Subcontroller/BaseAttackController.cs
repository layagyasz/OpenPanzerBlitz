using System;
using System.Linq;

using Cardamom.Utilities;

using SFML.Graphics;
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
			if (_AttackBuilder.Attackers.Count() == 0) CloseWindow(null, EventArgs.Empty);
		}

		public override void HandleKeyPress(Keyboard.Key Key) { }

		protected void SelectUnit(Unit Unit, AttackMethod AttackMethod)
		{
			_Controller.SelectUnit(Unit);
			_Controller.Highlight(
				Unit.GetFieldOfSight(AttackMethod).Select(
					i => new Tuple<Tile, Color>(i.Final, _Controller.GetRangeColor(i, Unit, AttackMethod))));
		}

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
					_AttackBuilder = attack;
					var attackPane = new AttackPane(_AttackBuilder);
					attackPane.OnClose = CloseWindow;
					attackPane.OnAttackTargetChanged += ChangeAttackTarget;
					attackPane.OnExecute += ExecuteAttack;

					_Controller.SetPane(attackPane);
				}
			}
			else _Controller.Alert(r);
		}

		void CloseWindow(object Sender, EventArgs E)
		{
			_Controller.Clear();
			_AttackBuilder = null;
		}

		void ExecuteAttack(object Sender, EventArgs E)
		{
			if (_Controller.ExecuteOrderAndAlert(_AttackBuilder)) CloseWindow(Sender, E);
		}

		void ChangeAttackTarget(object Sender, ValuedEventArgs<AttackTarget> E)
		{
			_AttackBuilder.SetAttackTarget(E.Value);
		}
	}
}
