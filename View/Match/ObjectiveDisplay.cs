using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class ObjectiveDisplay : GuiItem
	{
		readonly SingleColumnTable _Display = new SingleColumnTable("objective-display");

		public override Vector2f Size
		{
			get
			{
				return _Display.Size;
			}
		}

		public void SetVictoryCondition(VictoryCondition Condition, Army ForArmy, Match Match)
		{
			_Display.Clear();
			foreach (var objective in Condition.Scorers)
			{
				_Display.Add(
					new Button("objective-header")
					{
						DisplayedString =
							string.Format(
								"{0}/{1}",
								objective.CalculateScore(ForArmy, Match, new Dictionary<Objective, int>()),
								Condition.GetMaximumScore(objective, ForArmy, Match))
					});
				_Display.Add(
					new Button("objective-regular")
					{
						DisplayedString =
							ObjectDescriber.Sentencify(
								ObjectiveDescriber.RemoveScore(ObjectiveDescriber.Describe(objective)))
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
