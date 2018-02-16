using System;
using System.Collections.Generic;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class SelectPane<T> : Pane
	{
		public EventHandler<ValuedEventArgs<T>> OnItemSelected;

		Select<T> _UnitSelect = new Select<T>("select");

		public SelectPane(string Title, IEnumerable<T> Items)
			: base("select-pane")
		{
			var header = new Button("select-pane-header") { DisplayedString = Title };
			Add(header);

			_UnitSelect.Position = new Vector2f(0, header.Size.Y + 6);
			foreach (T item in Items)
			{
				var option = new SelectionOption<T>("select-option")
				{
					Value = item,
					DisplayedString = ObjectDescriber.Describe(item)
				};
				_UnitSelect.Add(option);
			}
			_UnitSelect.OnChange += HandleChange;
			Add(_UnitSelect);
		}

		void HandleChange(object sender, ValuedEventArgs<StandardItem<T>> E)
		{
			if (OnItemSelected != null) OnItemSelected(this, new ValuedEventArgs<T>(E.Value.Value));
		}
	}
}
