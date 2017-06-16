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

		ScrollCollection<Unit> _Selection;

		public Unit SelectedUnit
		{
			get
			{
				return _Selection.Value == null ? null : _Selection.Value.Value;
			}
		}

		public DeploymentPane(Army Army)
			: base("deployment-pane")
		{
			this.Army = Army;
			_Selection = new ScrollCollection<Unit>("deployment-select");
			foreach (Unit u in Army.Units) Add(u);
			Add(_Selection);
		}

		public void Add(Unit Unit)
		{
			_Selection.Add(new DeploymentSelectionOption(Unit));
		}

		public void Remove(Unit Unit)
		{
			_Selection.Remove(i => i.Value == Unit);
		}
	}
}
