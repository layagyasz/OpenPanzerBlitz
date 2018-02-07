﻿using System;
using System.Linq;

using Cardamom.Interface;

using SFML.Window;

namespace PanzerBlitz
{
	public class UnitGroupView : GuiContainer<GuiItem>
	{
		public UnitGroupView(UnitGroup UnitGroup, Faction Faction, UnitConfigurationRenderer Renderer)
			: base("scenario-select-deployment-units")
		{
			float offset = Math.Min(68, (Size.X - LeftPadding.X * 2 - 68) / (UnitGroup.UnitCounts.Count() - 1));
			int i = 0;
			foreach (UnitCount count in UnitGroup.UnitCounts)
			{
				Add(
					new UnitConfigurationStackView(
						count.UnitConfiguration,
						Faction,
						Renderer,
						64,
						"scenario-select-deployment-units-overlay",
						true)
					{
						Count = count.Count,
						Position = new Vector2f(offset * i + 32, 32)
					});
				++i;
			}
		}
	}
}
