using System;
using System.Linq;

using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class PositionalDeploymentPage : DeploymentPage
	{
		public EventHandler<EventArgs> OnSelectedStack;

		PositionalDeployment _Deployment;
		UnitConfigurationRenderer _Renderer;
		ScrollCollection<HomogenousStackView> _Selection;

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

			_Selection = new ScrollCollection<HomogenousStackView>("deployment-select");
			_Selection.OnChange +=
				(sender, e) => { if (OnSelectedStack != null) OnSelectedStack(this, EventArgs.Empty); };

			foreach (var g in Deployment.Units.GroupBy(i => i.Configuration))
				_Selection.Add(new PositionalDeploymentSelectionOption(g, _Renderer));

			_Selection.Position = new Vector2f(0, 48);
			Add(_Selection);
		}

		public void Add(Unit Unit)
		{
			PositionalDeploymentSelectionOption option =
							(PositionalDeploymentSelectionOption)_Selection.FirstOrDefault(
								i => ((PositionalDeploymentSelectionOption)i).UnitConfiguration == Unit.Configuration);
			if (option == null) _Selection.Add(new PositionalDeploymentSelectionOption(new Unit[] { Unit }, _Renderer));
			else option.Push(Unit);
		}

		public void Remove(Unit Unit)
		{
			PositionalDeploymentSelectionOption option =
				(PositionalDeploymentSelectionOption)_Selection.FirstOrDefault(
					i => ((PositionalDeploymentSelectionOption)i).UnitConfiguration == Unit.Configuration);
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
