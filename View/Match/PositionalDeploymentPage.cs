using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class PositionalDeploymentPage : DeploymentPage
	{
		public EventHandler<EventArgs> OnSelectedStack;

		PositionalDeployment _Deployment;
		UnitConfigurationRenderer _Renderer;
		ValuedScrollCollection<GroupedUnitSelectionOption, HomogenousStackView> _Selection;

		public override Deployment Deployment
		{
			get
			{
				return _Deployment;
			}
		}

		public HomogenousStackView SelectedStack
		{
			get
			{
				if (_Selection.Value == null) return null;
				return _Selection.Value.Value;
			}
		}

		public PositionalDeploymentPage(
			PositionalDeployment Deployment, UnitConfigurationRenderer Renderer, DeploymentPane Pane)
		{
			_Deployment = Deployment;
			_Renderer = Renderer;

			_Selection =
				new ValuedScrollCollection<GroupedUnitSelectionOption, HomogenousStackView>("deployment-select");
			_Selection.OnChange +=
				(sender, e) => { if (OnSelectedStack != null) OnSelectedStack(this, EventArgs.Empty); };

			foreach (var g in Deployment.Units.GroupBy(i => i.Configuration))
				_Selection.Add(
					new GroupedUnitSelectionOption(
						"deployment-selection-option", "deployment-selection-option-overlay", g, _Renderer));

			_Selection.Position = new Vector2f(0, 48);
			Add(_Selection);
		}

		public void Add(Unit Unit)
		{
			GroupedUnitSelectionOption option =
				_Selection.FirstOrDefault(i => i.UnitConfiguration == Unit.Configuration);
			if (option == null) _Selection.Add(
				new GroupedUnitSelectionOption(
					"deployment-selection-option",
					"deployment-selection-option-overlay",
					new Unit[] { Unit },
					_Renderer));
			else option.Push(Unit);
		}

		public void Remove(Unit Unit)
		{
			GroupedUnitSelectionOption option =
				_Selection.FirstOrDefault(i => i.UnitConfiguration == Unit.Configuration);
			if (option != null) option.Pop();
			if (option.Count == 0) _Selection.Remove(option);
		}

		public Unit Peek()
		{
			if (SelectedStack != null) return SelectedStack.Peek();
			return null;
		}
	}
}
