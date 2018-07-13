namespace PanzerBlitz
{
	public static class DeploymentDescriber
	{
		public static string Describe(DeploymentConfiguration Deployment)
		{
			if (Deployment is PositionalDeploymentConfiguration)
				return Describe((PositionalDeploymentConfiguration)Deployment);
			if (Deployment is ConvoyDeploymentConfiguration) return Describe((ConvoyDeploymentConfiguration)Deployment);
			return string.Empty;
		}

		public static string Describe(PositionalDeploymentConfiguration Deployment)
		{
			return string.Format("Set up {0}.", MatcherDescriber.Describe(Deployment.Matcher));
		}

		public static string Describe(ConvoyDeploymentConfiguration Deployment)
		{
			var description = MatcherDescriber.Describe(Deployment.Matcher);
			if (Deployment.EntryTurn == 0) return string.Format("Enter {0}.", description);
			return string.Format("Enter {0} on turn {1}.", description, Deployment.EntryTurn);
		}
	}
}
