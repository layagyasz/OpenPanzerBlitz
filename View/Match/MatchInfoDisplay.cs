using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MatchInfoDisplay : GuiItem
	{
		SingleColumnTable _InfoDisplay = new SingleColumnTable("info-display");
		Pod _ViewItem;

		public override Vector2f Size
		{
			get
			{
				return _InfoDisplay.Size;
			}
		}

		public void SetTurn(Turn Turn)
		{
			_InfoDisplay.Clear();
			_InfoDisplay.Add(
				new Button("info-display-header") { DisplayedString = ObjectDescriber.Describe(Turn.TurnInfo.Army) });
			_InfoDisplay.Add(
				new Button("info-display-info")
				{
					DisplayedString = ObjectDescriber.Describe(Turn.TurnInfo.TurnComponent)
				});
		}

		public void SetViewItem(Pod ViewItem)
		{
			_ViewItem = ViewItem;
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);
			Transform.Translate(Position);
			_InfoDisplay.Update(MouseController, KeyController, DeltaT, Transform);
			if (_ViewItem != null)
			{
				Transform.Translate(_InfoDisplay.LeftPadding);
				_ViewItem.Update(MouseController, KeyController, DeltaT, Transform);
			}
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			_InfoDisplay.Draw(Target, Transform);
			if (_ViewItem != null)
			{
				Transform.Translate(_InfoDisplay.LeftPadding);
				_ViewItem.Draw(Target, Transform);
			}
		}
	}
}
