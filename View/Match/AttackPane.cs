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
		public EventHandler<EventArgs> OnClose;
		public EventHandler<ValuedEventArgs<AttackTarget>> OnAttackTargetChanged;
		public EventHandler<EventArgs> OnExecute;

		Button _CloseButton = new Button("attack-close-button") { DisplayedString = "X" };
		ScrollCollection<ClassedGuiItem> _Description = new ScrollCollection<ClassedGuiItem>("attack-display");
		Select<AttackTarget> _AttackTargetSelect = new Select<AttackTarget>("attack-target-select");
		Button _OrderButton = new Button("large-button") { DisplayedString = "Engage" };

		public readonly AttackOrder Attack;

		public AttackPane(AttackOrder Attack)
			: base("attack-pane")
		{
			this.Attack = Attack;
			Attack.OnChanged += UpdateDescription;

			_CloseButton.Position = new Vector2f(Size.X - _CloseButton.Size.X - LeftPadding.X * 2, 0);
			_CloseButton.OnClick += HandleClose;

			foreach (var target in Enum.GetValues(typeof(AttackTarget)).Cast<AttackTarget>())
			{
				_AttackTargetSelect.Add(new SelectionOption<AttackTarget>("attack-target-select-option")
				{
					DisplayedString = ObjectDescriber.Describe(target),
					Value = target
				});
			}
			_AttackTargetSelect.SetValue(i => i.Value == Attack.Target);
			_AttackTargetSelect.OnChange += HandleAttackTargetChanged;
			_OrderButton.Position = new Vector2f(0, Size.Y - _OrderButton.Size.Y - 32);
			_OrderButton.OnClick += (sender, e) => { if (OnExecute != null) OnExecute(this, EventArgs.Empty); };
			_Description.Position = new Vector2f(0, _AttackTargetSelect.Size.Y + 24);

			Add(_CloseButton);
			Add(_Description);
			Add(_AttackTargetSelect);
			Add(_OrderButton);

			UpdateDescription(null, EventArgs.Empty);
		}

		void HandleClose(object Sender, EventArgs E)
		{
			OnClose?.Invoke(this, EventArgs.Empty);
		}

		void UpdateDescription(object Sender, EventArgs E)
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

			var p = Attack.CombatResultsTable.GetCombatResultProbabilities(OddsCalculation);
			for (int i = 0; i < p.Length; ++i)
			{
				if (p[i] > 0)
				{
					_Description.Add(new Button("attack-factor-box")
					{
						DisplayedString = string.Format(
							"{0} - {1}%", ObjectDescriber.Describe((CombatResult)i), Math.Round(p[i] * 100))
					});
				}
			}

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
					DisplayedString = string.Format("+{0} {1}", d.Configuration.Defense, d.Configuration.Name)
				});
			_Description.Add(new Button("odds-factor-box")
			{
				DisplayedString = OddsCalculation.StackArmored ? "Armored Target" : "Unarmored Target"
			});
			foreach (OddsCalculationFactor o in OddsCalculation.OddsCalculationFactors)
				_Description.Add(new Button("odds-factor-box") { DisplayedString = ObjectDescriber.Describe(o) });
		}

		void DescribeAttackFactorCalculation(
			SingleAttackOrder Attacker, AttackFactorCalculation AttackFactorCalculation)
		{
			_Description.Add(new Button("attack-part-box")
			{
				DisplayedString = string.Format(
					"+{0} {1}", AttackFactorCalculation.Attack, ObjectDescriber.Describe(Attacker.Attacker))
			});
			_Description.Add(new Button("attack-factor-box")
			{
				DisplayedString = string.Format(
					"{0} Base Attack Factor",
					Attacker.Attacker.Configuration.GetWeapon(Attacker.UseSecondaryWeapon).Attack)
			});
			foreach (AttackFactorCalculationFactor a in AttackFactorCalculation.Factors)
				_Description.Add(new Button("attack-factor-box") { DisplayedString = ObjectDescriber.Describe(a) });
		}

		string OddsString(OddsCalculation OddsCalculation)
		{
			var dieModifier =
				string.Format(
					"{0} {1}", OddsCalculation.DieModifier < 0 ? "-" : "+", Math.Abs(OddsCalculation.DieModifier));
			if (OddsCalculation.OddsAgainst)
				return string.Format("Attack at 1-{0} {1} against", OddsCalculation.Odds, dieModifier);
			return string.Format("Attack at {0}-1 {1} for", OddsCalculation.Odds, dieModifier);
		}

		void HandleAttackTargetChanged(object Sender, ValuedEventArgs<StandardItem<AttackTarget>> E)
		{
			if (OnAttackTargetChanged != null)
				OnAttackTargetChanged(this, new ValuedEventArgs<AttackTarget>(E.Value.Value));
		}
	}
}
