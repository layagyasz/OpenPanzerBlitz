using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public abstract class DeploymentPage : Container<GuiItem>
	{
		public abstract Deployment Deployment { get; }
	}
}
