using System;
using System.Linq;

using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class AttackPane : Pane
	{
		ScrollCollection<object> _Description = new ScrollCollection<object>("attack-display");
		Select<AttackTarget> _AttackTargetSelect = new Select<AttackTarget>("select")
		{
			Position = new Vector2f(16, 16)
		};
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
			_OrderButton.Position = Size - _OrderButton.Size - new Vector2f(16, 16);
			_Description.Position = new Vector2f(0, _AttackTargetSelect.Size.Y + 24);
			Add(_Description);
			Add(_AttackTargetSelect);
			Add(_OrderButton);
		}

		public void UpdateDescription()
		{
			_Description.Clear();
			foreach (OddsCalculation o in Attack.OddsCalculations) DescribeOddsCalculation(o);
		}

		private void DescribeOddsCalculation(OddsCalculation OddsCalculation)
		{
			_Description.Add(new Button("attack-odds-box") { DisplayedString = OddsString(OddsCalculation) });
			_Description.Add(new Button("attack-odds-section")
			{
				DisplayedString = string.Format("{0} Total Attack Factor", OddsCalculation.TotalAttack)
			});
			foreach (var a in OddsCalculation.AttackFactorCalculations)
				DescribeAttackFactorCalculation(a.Item1, a.Item2);
			_Description.Add(new Button("attack-odds-section")
			{
				DisplayedString = string.Format("{0} Total Defense Factor", OddsCalculation.TotalDefense)
			});
			foreach (Unit d in OddsCalculation.Defenders)
				_Description.Add(new Button("attack-part-box")
				{
					DisplayedString = string.Format("+{0} {1}", d.UnitConfiguration.Defense, d.UnitConfiguration.Name)
				});
			_Description.Add(new Button("odds-factor-box")
			{
				DisplayedString = OddsCalculation.StackArmored ? "Armored Target" : "Unarmored Target"
			});
			foreach (OddsCalculationFactor o in OddsCalculation.OddsCalculationFactors)
				_Description.Add(new Button("odds-factor-box") { DisplayedString = o.ToString() });
		}

		private void DescribeAttackFactorCalculation(Unit Unit, AttackFactorCalculation AttackFactorCalculation)
		{
			_Description.Add(new Button("attack-part-box")
			{
				DisplayedString = string.Format(
					"+{0} {1}", AttackFactorCalculation.Attack, Unit.UnitConfiguration.Name)
			});
			_Description.Add(new Button("attack-factor-box")
			{
				DisplayedString = string.Format("{0} Base Attack Factor", Unit.UnitConfiguration.Attack)
			});
			foreach (AttackFactorCalculationFactor a in AttackFactorCalculation.Factors)
				_Description.Add(new Button("attack-factor-box") { DisplayedString = a.ToString() });
		}

		private string OddsString(OddsCalculation OddsCalculation)
		{
			if (OddsCalculation.OddsAgainst) return string.Format("Attack at 1-{0} against", OddsCalculation.Odds);
			else return string.Format("Attack at {0}-1 for", OddsCalculation.Odds);
		}
	}
}
