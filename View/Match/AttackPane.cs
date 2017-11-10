using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class AttackPane : Pane
	{
		public EventHandler<ValuedEventArgs<AttackTarget>> OnAttackTargetChanged;
		public EventHandler<EventArgs> OnExecute;

		ScrollCollection<ClassedGuiItem> _Description = new ScrollCollection<ClassedGuiItem>("attack-display");
		Select<AttackTarget> _AttackTargetSelect = new Select<AttackTarget>("select");
		Button _OrderButton = new Button("large-button") { DisplayedString = "Engage" };

		public readonly AttackOrder Attack;

		public AttackPane(AttackOrder Attack)
			: base("attack-pane")
		{
			this.Attack = Attack;
			_AttackTargetSelect.Add(new SelectionOption<AttackTarget>("select-option")
			{
				DisplayedString = AttackTarget.ALL.ToString(),
				Value = AttackTarget.ALL
			});
			_AttackTargetSelect.Add(new SelectionOption<AttackTarget>("select-option")
			{
				DisplayedString = AttackTarget.WEAKEST.ToString(),
				Value = AttackTarget.WEAKEST
			});
			_AttackTargetSelect.Add(new SelectionOption<AttackTarget>("select-option")
			{
				DisplayedString = AttackTarget.EACH.ToString(),
				Value = AttackTarget.EACH
			});
			_AttackTargetSelect.OnChange += HandleAttackTargetChanged;
			_OrderButton.Position = new Vector2f(0, Size.Y - _OrderButton.Size.Y - 32);
			_OrderButton.OnClick += (sender, e) => { if (OnExecute != null) OnExecute(this, EventArgs.Empty); };
			_Description.Position = new Vector2f(0, _AttackTargetSelect.Size.Y + 24);
			Add(_Description);
			Add(_AttackTargetSelect);
			Add(_OrderButton);
		}

		public void UpdateDescription()
		{
			_Description.Clear();
			foreach (OddsCalculation o in Attack.OddsCalculations) DescribeOddsCalculation(o);
			if (Attack.Validate() != OrderInvalidReason.NONE)
				_Description.Add(new Button("attack-error-box")
				{
					DisplayedString = Attack.Validate().ToString()
				});
		}

		void DescribeOddsCalculation(OddsCalculation OddsCalculation)
		{
			_Description.Add(new Button("attack-odds-box") { DisplayedString = OddsString(OddsCalculation) });
			_Description.Add(new Button("attack-odds-section")
			{
				DisplayedString = string.Format("{0} Total Attack Factor", OddsCalculation.TotalAttack)
			});
			foreach (var a in OddsCalculation.AttackFactorCalculations)
				DescribeAttackFactorCalculation(a.Item1.Attacker, a.Item2);
			_Description.Add(new Button("attack-odds-section")
			{
				DisplayedString = string.Format("{0} Total Defense Factor", OddsCalculation.TotalDefense)
			});
			foreach (Unit d in OddsCalculation.Defenders)
				_Description.Add(new Button("attack-part-box")
				{
					DisplayedString = string.Format("+{0} {1}", d.Configuration.Defense, d.Configuration.Name)
				});
			_Description.Add(new Button("odds-factor-box")
			{
				DisplayedString = OddsCalculation.StackArmored ? "Armored Target" : "Unarmored Target"
			});
			foreach (OddsCalculationFactor o in OddsCalculation.OddsCalculationFactors)
				_Description.Add(new Button("odds-factor-box") { DisplayedString = o.ToString() });
		}

		void DescribeAttackFactorCalculation(Unit Unit, AttackFactorCalculation AttackFactorCalculation)
		{
			_Description.Add(new Button("attack-part-box")
			{
				DisplayedString = string.Format(
					"+{0} {1}", AttackFactorCalculation.Attack, Unit.Configuration.Name)
			});
			_Description.Add(new Button("attack-factor-box")
			{
				DisplayedString = string.Format("{0} Base Attack Factor", Unit.Configuration.Attack)
			});
			foreach (AttackFactorCalculationFactor a in AttackFactorCalculation.Factors)
				_Description.Add(new Button("attack-factor-box") { DisplayedString = a.ToString() });
		}

		string OddsString(OddsCalculation OddsCalculation)
		{
			string dieModifier =
				string.Format(
					"{0} {1}", OddsCalculation.DieModifier < 0 ? "-" : "+", Math.Abs(OddsCalculation.DieModifier));
			if (OddsCalculation.OddsAgainst)
				return string.Format("Attack at 1-{0} {1} against", OddsCalculation.Odds, dieModifier);
			else return string.Format("Attack at {0}-1 {1} for", OddsCalculation.Odds, dieModifier);
		}

		void HandleAttackTargetChanged(object Sender, ValuedEventArgs<StandardItem<AttackTarget>> E)
		{
			if (OnAttackTargetChanged != null)
				OnAttackTargetChanged(this, new ValuedEventArgs<AttackTarget>(E.Value.Value));
		}
	}
}
