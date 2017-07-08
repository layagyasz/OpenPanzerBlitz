using System;
namespace PanzerBlitz
{
	public abstract class BaseAttackController<T> : BaseController where T : AttackOrder
	{
		protected T _AttackBuilder;
		protected AttackPane _AttackPane;

		public BaseAttackController(Match Match, GameScreen GameScreen)
			: base(Match, GameScreen)
		{
		}

		public override void End()
		{
			base.End();

			if (_AttackBuilder != null) _GameScreen.RemovePane(_AttackPane);
		}

		protected void StartAttack(T Attack)
		{
			if (_AttackBuilder != null) _GameScreen.RemovePane(_AttackPane);

			_AttackBuilder = Attack;
			_AttackPane = new AttackPane(_AttackBuilder);
			_GameScreen.AddPane(_AttackPane);
			_AttackPane.UpdateDescription();
			_AttackPane.OnExecute += ExecuteAttack;
		}

		private void ExecuteAttack(object sender, EventArgs e)
		{
			if (_Match.ExecuteOrder(_AttackBuilder)) _GameScreen.RemovePane(_AttackPane);
			else _AttackPane.UpdateDescription();
		}
	}
}
