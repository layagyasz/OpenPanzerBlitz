using System;
namespace PanzerBlitz
{
	public abstract class DeploymentMicrocontroller : BaseController
	{
		public abstract Deployment Deployment { get; }

		public DeploymentMicrocontroller(Match Match, GameScreen GameScreen)
			: base(Match, GameScreen)
		{
		}

		public abstract DeploymentPage MakePage(DeploymentPane Pane, UnitConfigurationRenderer Renderer);
	}
}
