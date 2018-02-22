using Cardamom.Interface.Items;

namespace PanzerBlitz
{
	public class VictoryConditionRow : SingleColumnTable
	{
		public VictoryConditionRow(ObjectiveSuccessTrigger Trigger)
			: base("scenario-victory-condition-row")
		{
			Add(new Button("scenario-victory-condition-header")
			{
				DisplayedString = ObjectDescriber.Describe(Trigger.SuccessLevel)
			});
			Add(new Button("scenario-victory-condition")
			{
				DisplayedString = ObjectDescriber.Describe(Trigger)
			});
		}
	}
}
