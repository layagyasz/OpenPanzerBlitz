using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class UnitConfigurationTable : Table
	{
		public EventHandler<ValuedEventArgs<UnitConfiguration>> OnUnitClicked;
		public EventHandler<ValuedEventArgs<UnitConfiguration>> OnUnitRightClicked;

		UnitConfigurationRenderer _Renderer;
		Faction _Faction;

		string _RowClassName;
		string _CellClassName;
		int _ItemWidth;
		bool _StackDuplicates;

		public UnitConfigurationTable(
			string ClassName,
			string RowClassName,
			string CellClassName,
			Faction Faction,
			UnitConfigurationRenderer Renderer,
			bool StackDuplicates)
			: base(ClassName)
		{
			_Renderer = Renderer;
			_Faction = Faction;
			_RowClassName = RowClassName;
			_CellClassName = CellClassName;
			_ItemWidth = _Class.GetAttributeWithDefault("item-width", 1);
			_StackDuplicates = StackDuplicates;
		}

		public void Add(UnitConfiguration UnitConfiguration)
		{
			if (_StackDuplicates)
			{
				UnitConfigurationSelectionOption o =
					_Items
						.SelectMany(i => i)
						.Cast<UnitConfigurationSelectionOption>()
						.FirstOrDefault(i => i.UnitConfiguration == UnitConfiguration);
				if (o != null)
				{
					o.StackView.Count++;
					return;
				}
			}

			TableRow t = null;
			if (_Items.Count == 0 || _Items.Last().Count() >= _ItemWidth)
			{
				t = new TableRow(_RowClassName);
				_Items.Add(t);
			}
			else t = _Items.Last();

			UnitConfigurationSelectionOption option =
				new UnitConfigurationSelectionOption(
					_CellClassName,
					_CellClassName + "-details",
					_CellClassName + "-overlay",
					UnitConfiguration,
					_Faction,
					_Renderer,
					_StackDuplicates);
			option.OnClick += HandleClick;
			option.OnRightClick += HandleRightClick;
			t.Add(option);
		}

		public void Remove(UnitConfiguration UnitConfiguration)
		{
			TableRow current = null;
			List<TableRow> rows = _Items.ToList();
			_Items.Clear();
			foreach (TableRow row in rows)
			{
				foreach (UnitConfigurationSelectionOption option in row)
				{
					if (option.UnitConfiguration != UnitConfiguration
						|| (_StackDuplicates && --option.StackView.Count > 0))
					{
						if (current == null || current.Count() >= _ItemWidth)
						{
							current = new TableRow(_RowClassName);
							_Items.Add(current);
						}
						current.Add(option);
					}
				}
			}
		}

		void HandleClick(object Sender, EventArgs E)
		{
			if (OnUnitClicked != null)
				OnUnitClicked(
					this,
					new ValuedEventArgs<UnitConfiguration>(
						((UnitConfigurationSelectionOption)Sender).UnitConfiguration));
		}

		void HandleRightClick(object Sender, EventArgs E)
		{
			if (OnUnitRightClicked != null)
				OnUnitRightClicked(
					this,
					new ValuedEventArgs<UnitConfiguration>(
						((UnitConfigurationSelectionOption)Sender).UnitConfiguration));
		}
	}
}
