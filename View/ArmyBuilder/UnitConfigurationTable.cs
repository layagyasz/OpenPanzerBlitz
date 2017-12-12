using System;
using System.Linq;

using Cardamom.Interface.Items;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class UnitConfigurationTable : Table
	{
		UnitConfigurationRenderer _Renderer;
		Faction _Faction;

		string _RowClassName;
		string _CellClassName;
		int _ItemWidth;

		public UnitConfigurationTable(
			string ClassName,
			string RowClassName,
			string CellClassName,
			Faction Faction,
			UnitConfigurationRenderer Renderer)
			: base(ClassName)
		{
			_Renderer = Renderer;
			_Faction = Faction;
			_RowClassName = RowClassName;
			_CellClassName = CellClassName;
			_ItemWidth = _Class.GetAttributeWithDefault("item-width", 1);
		}

		public void Add(UnitConfiguration UnitConfiguration)
		{
			TableRow t = null;
			if (_Items.Count == 0 || _Items.Last().Count() >= _ItemWidth)
			{
				t = new TableRow(_RowClassName);
				_Items.Add(t);
			}
			else t = _Items.Last();

			t.Add(new UnitConfigurationSelectionOption(_CellClassName, UnitConfiguration, _Faction, _Renderer));
		}
	}
}
