using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;


namespace PanzerBlitz
{
	public class DeploymentPane : Pane
	{
		public readonly Army Army;

		UnitConfigurationRenderer _Renderer;

		ScrollCollection<HomogenousStackView> _Selection;

		public DeploymentPane(Army Army, UnitConfigurationRenderer Renderer)
					: base("deployment-pane")
		{
			this.Army = Army;
			_Renderer = Renderer;

			_Selection = new ScrollCollection<HomogenousStackView>("deployment-select");

			foreach (var g in Army.Units.GroupBy(i => i.UnitConfiguration))
				_Selection.Add(new DeploymentSelectionOption(g, _Renderer));

			Add(_Selection);
		}

		public void Add(Unit Unit)
		{
			DeploymentSelectionOption option =
							(DeploymentSelectionOption)_Selection.FirstOrDefault(
								i => ((DeploymentSelectionOption)i).UnitConfiguration == Unit.UnitConfiguration);
			if (option == null) _Selection.Add(new DeploymentSelectionOption(new Unit[] { Unit }, _Renderer));
			else option.Push(Unit);
		}

		public void Remove(Unit Unit)
		{
			DeploymentSelectionOption option =
				(DeploymentSelectionOption)_Selection.FirstOrDefault(
					i => ((DeploymentSelectionOption)i).UnitConfiguration == Unit.UnitConfiguration);
			if (option != null) option.Pop();
			if (option.Count == 0) _Selection.Remove(option);
		}

		public Unit Peek()
		{
			if (_Selection.Value != null) return _Selection.Value.Value.Peek();
			return null;
		}
	}
}
