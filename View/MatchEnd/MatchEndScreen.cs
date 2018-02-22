using System;

using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class MatchEndScreen : ScreenBase
	{
		public EventHandler<EventArgs> OnSaveClicked;

		SingleColumnTable _Table = new SingleColumnTable("match-end-display");
		Button _SaveButton = new Button("small-button") { DisplayedString = "Save Record" };

		public MatchEndScreen(Match Match, Vector2f WindowSize, FactionRenderer FactionRenderer)
			: base(WindowSize)
		{
			_Table.Position = .5f * (WindowSize - _Table.Size);
			foreach (Army a in Match.Armies)
			{
				_Table.Add(new MatchEndArmyRow(a, FactionRenderer));
			}

			_SaveButton.Position = new Vector2f(_Table.Size.X - _SaveButton.Size.X, 0);
			_SaveButton.OnClick += HandleSaveClicked;
			_Table.Add(_SaveButton);

			_Items.Add(_Table);
		}

		void HandleSaveClicked(object Sender, EventArgs E)
		{
			if (OnSaveClicked != null) OnSaveClicked(this, E);
		}
	}
}
