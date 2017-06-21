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

		ScrollCollection<Unit> _Selection;

		public Unit SelectedUnit
		{
			get
			{
				return _Selection.Value == null ? null : _Selection.Value.Value;
			}
		}

		public DeploymentPane(Army Army, UnitConfigurationRenderer Renderer)
			: base("deployment-pane")
		{
			this.Army = Army;
			_Renderer = Renderer;

			_Selection = new ScrollCollection<Unit>("deployment-select");
			foreach (Unit u in Army.Units.Where(i => i.Position == null)) Add(u);
			Add(_Selection);
		}

		public void Add(Unit Unit)
		{
			_Selection.Add(new DeploymentSelectionOption(Unit, _Renderer));
		}

		public void Remove(Unit Unit)
		{
			_Selection.Remove(i => i.Value == Unit);
		}
	}
}
