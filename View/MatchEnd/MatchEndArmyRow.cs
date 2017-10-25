﻿using System;

using Cardamom.Interface.Items;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class MatchEndArmyRow : SingleColumnTable
	{
		FactionView _FactionView;

		public MatchEndArmyRow(Match Match, Army Army)
			: base("match-end-army-row")
		{
			_FactionView = new FactionView(Army.Configuration.Faction, 128) { Position = LeftPadding };
			Add(new Button("match-end-army-header")
			{
				DisplayedString = ObjectDescriber.Describe(Army.Configuration.Faction)
			});
			Add(new Button("match-end-army-success")
			{
				DisplayedString = ObjectDescriber.Describe(Army.GetObjectiveSuccessLevel(Match))
			});
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);
			_FactionView.Draw(Target, Transform);
		}
	}
}