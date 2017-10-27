using System;
namespace PanzerBlitz
{
	public abstract class DeploymentMicrocontroller : BaseController
	{
		public abstract Deployment Deployment { get; }

		public DeploymentMicrocontroller(HumanMatchPlayerController Controller)
			: base(Controller)
		{
		}

		public abstract DeploymentPage MakePage(DeploymentPane Pane, UnitConfigurationRenderer Renderer);
	}
}
