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
			if (_AttackBuilder != null)
			{
				_AttackBuilder.RemoveAttacker(Unit);
				_AttackPane.UpdateDescription();
			}
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
		}

		protected void AddAttack(Army Army, Tile Tile, AttackMethod AttackMethod, SingleAttackOrder NewAttack)
		{
			AttackOrder attack = _AttackBuilder == null || _AttackBuilder.AttackAt != Tile
																		 ? new AttackOrder(Army, Tile, AttackMethod)
																		 : _AttackBuilder;
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
				_AttackPane.UpdateDescription();
			}
			else _Controller.Alert(r);
		}

		protected Color GetRangeColor(Color[] ColorSet, int Range, int MaxRange)
		{
			return ColorSet[Math.Min(Range * HIGHLIGHT_COLORS.Length / (MaxRange + 1), ColorSet.Length - 1)];
		}

		void ExecuteAttack(object sender, EventArgs e)
		{
			if (_Controller.ExecuteOrderAndAlert(_AttackBuilder))
			{
				_Controller.RemovePane(_AttackPane);
				_AttackBuilder = null;
			}
			else _AttackPane.UpdateDescription();
		}

		void ChangeAttackTarget(object Sender, ValuedEventArgs<AttackTarget> E)
		{
			_AttackBuilder.SetAttackTarget(E.Value);
			_AttackPane.UpdateDescription();
		}
	}
}
