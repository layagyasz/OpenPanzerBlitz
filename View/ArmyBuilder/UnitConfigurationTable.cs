using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class UnitConfigurationTable : Table
	{
		public EventHandler<ValuedEventArgs<UnitConfigurationLink>> OnUnitClicked;
		public EventHandler<ValuedEventArgs<UnitConfigurationLink>> OnUnitRightClicked;

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

		public void Add(UnitConfigurationLink UnitConfigurationLink)
		{
			if (_StackDuplicates)
			{
				UnitConfigurationSelectionOption o =
					_Items
						.SelectMany(i => i)
						.Cast<UnitConfigurationSelectionOption>()
						.FirstOrDefault(i => i.UnitConfigurationLink == UnitConfigurationLink);
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
					UnitConfigurationLink,
					_Faction,
					_Renderer,
					_StackDuplicates);
			option.OnClick += HandleClick;
			option.OnRightClick += HandleRightClick;
			t.Add(option);
		}

		public IEnumerable<Tuple<UnitConfigurationLink, int>> GetUnitConfigurationLinks()
		{
			return _Items
				.SelectMany(i => i)
				.Cast<UnitConfigurationSelectionOption>()
				.Select(i => new Tuple<UnitConfigurationLink, int>(i.UnitConfigurationLink, i.StackView.Count));
		}

		public void Remove(UnitConfigurationLink UnitConfigurationLink)
		{
			TableRow current = null;
			List<TableRow> rows = _Items.ToList();
			_Items.Clear();
			foreach (TableRow row in rows)
			{
				foreach (UnitConfigurationSelectionOption option in row)
				{
					if (option.UnitConfigurationLink != UnitConfigurationLink
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
					new ValuedEventArgs<UnitConfigurationLink>(
						((UnitConfigurationSelectionOption)Sender).UnitConfigurationLink));
		}

		void HandleRightClick(object Sender, EventArgs E)
		{
			if (OnUnitRightClicked != null)
				OnUnitRightClicked(
					this,
					new ValuedEventArgs<UnitConfigurationLink>(
						((UnitConfigurationSelectionOption)Sender).UnitConfigurationLink));
		}
	}
}
