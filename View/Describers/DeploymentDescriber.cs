namespace PanzerBlitz
{
	public static class DeploymentDescriber
	{
		public static string Describe(DeploymentConfiguration Deployment)
		{
			if (Deployment is PositionalDeploymentConfiguration)
				return Describe((PositionalDeploymentConfiguration)Deployment);
			if (Deployment is ConvoyDeploymentConfiguration) return Describe((ConvoyDeploymentConfiguration)Deployment);
			return "";
		}

		public static string Describe(PositionalDeploymentConfiguration Deployment)
		{
			return string.Format("Set up {0}", MatcherDescriber.Describe(Deployment.Matcher));
		}

		public static string Describe(ConvoyDeploymentConfiguration Deployment)
		{
			return string.Format("Enter {0}", MatcherDescriber.Describe(Deployment.Matcher));
		}
	}
}
