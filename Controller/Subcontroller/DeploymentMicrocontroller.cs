using System;
namespace PanzerBlitz
{
	public abstract class DeploymentMicrocontroller : BaseController
	{
		public abstract DeploymentPage DeploymentPage { get; }
		public abstract Deployment Deployment { get; }

		public DeploymentMicrocontroller(Match Match, GameScreen GameScreen)
			: base(Match, GameScreen)
		{
		}
	}
}
