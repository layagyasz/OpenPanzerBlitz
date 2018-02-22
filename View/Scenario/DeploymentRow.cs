using Cardamom.Interface.Items;

namespace PanzerBlitz
{
	public class DeploymentRow : SingleColumnTable
	{
		public DeploymentRow(DeploymentConfiguration Deployment, Faction Faction, UnitConfigurationRenderer Renderer)
			: base("scenario-deployment-row")
		{
			Add(new Button("scenario-deployment-header")
			{
				DisplayedString = Deployment.UnitGroup.Name
			});
			Add(new Button("scenario-deployment")
			{
				DisplayedString = DeploymentDescriber.Describe(Deployment)
			});
			Add(new UnitGroupView(Deployment.UnitGroup, Faction, Renderer));
		}
	}
}
