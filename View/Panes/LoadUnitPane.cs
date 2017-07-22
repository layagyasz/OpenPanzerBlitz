using System;
using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class LoadUnitPane : Pane
	{
		public EventHandler<ValueChangedEventArgs<Unit>> OnUnitSelected;

		Select<Unit> _UnitSelect = new Select<Unit>("select");

		public LoadUnitPane(IEnumerable<Unit> Units)
			: base("load-unit-pane")
		{
			Button header = new Button("load-unit-header") { DisplayedString = "Select Unit" };
			Add(header);

			_UnitSelect.Position = new Vector2f(0, header.Size.Y + 6);
			foreach (Unit u in Units)
			{
				SelectionOption<Unit> option = new SelectionOption<Unit>("select-option")
				{
					Value = u,
					DisplayedString = u.Configuration.Name
				};
				_UnitSelect.Add(option);
			}
			_UnitSelect.OnChange += HandleChange;
			Add(_UnitSelect);
		}

		void HandleChange(object sender, ValueChangedEventArgs<StandardItem<Unit>> E)
		{
			if (OnUnitSelected != null) OnUnitSelected(this, new ValueChangedEventArgs<Unit>(E.Value.Value));
		}
	}
}
