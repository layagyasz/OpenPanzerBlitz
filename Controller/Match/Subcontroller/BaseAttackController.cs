using System;

using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public abstract class BaseAttackController : BaseController
	{
		protected AttackOrder _AttackBuilder;
		protected AttackPane _AttackPane;

		public BaseAttackController(MatchAdapter Match, MatchScreen GameScreen)
			: base(Match, GameScreen)
		{
		}

		void Clear()
		{
			if (_AttackBuilder != null)
			{
				_GameScreen.PaneLayer.Remove(_AttackPane);
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

		protected void StartAttack(AttackOrder Attack)
		{
			Clear();

			_AttackBuilder = Attack;
			_AttackPane = new AttackPane(_AttackBuilder);
			_GameScreen.PaneLayer.Add(_AttackPane);
			_AttackPane.UpdateDescription();
			_AttackPane.OnAttackTargetChanged += ChangeAttackTarget;
			_AttackPane.OnExecute += ExecuteAttack;
		}

		void ExecuteAttack(object sender, EventArgs e)
		{
			if (_Match.ExecuteOrder(_AttackBuilder))
			{
				_GameScreen.PaneLayer.Remove(_AttackPane);
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
