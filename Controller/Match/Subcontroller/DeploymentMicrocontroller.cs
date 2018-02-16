namespace PanzerBlitz
{
	public abstract class DeploymentMicrocontroller : BaseController
	{
		public abstract Deployment Deployment { get; }

		protected DeploymentMicrocontroller(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public abstract DeploymentPage MakePage(DeploymentPane Pane, UnitConfigurationRenderer Renderer);
	}
}
