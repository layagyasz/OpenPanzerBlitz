using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class VictoryConditionDisplay : GuiItem
	{
		SingleColumnTable _Display = new SingleColumnTable("victory-condition-display");

		public override Vector2f Size
		{
			get
			{
				return _Display.Size;
			}
		}

		public void SetVictoryCondition(VictoryCondition Condition)
		{
			_Display.Clear();
			foreach (ObjectiveSuccessTrigger trigger in Condition.Triggers)
			{
				_Display.Add(
					new Button("victory-condition-header")
					{
						DisplayedString = ObjectDescriber.Describe(trigger.SuccessLevel)
					});
				_Display.Add(
					new Button("victory-condition-regular")
					{
						DisplayedString =
							ObjectDescriber.Sentencify(
								ObjectiveDescriber.Describe(
									new TriggerObjective(trigger.Objective, trigger.Threshold, trigger.Invert)))
					});
			}
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			Transform.Translate(Position);
			_Display.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			_Display.Draw(Target, Transform);
		}
	}
}
