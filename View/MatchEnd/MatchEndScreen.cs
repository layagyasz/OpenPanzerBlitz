using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MatchEndScreen : ScreenBase
	{
		SingleColumnTable _Table = new SingleColumnTable("match-end-display");

		public MatchEndScreen(Match Match, Vector2f WindowSize)
			: base(WindowSize)
		{
			_Table.Position = .5f * (WindowSize - _Table.Size);
			foreach (Army a in Match.Armies)
			{
				_Table.Add(new MatchEndArmyRow(Match, a));
			}
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);
			_Table.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);
			_Table.Draw(Target, Transform);
		}
	}
}
