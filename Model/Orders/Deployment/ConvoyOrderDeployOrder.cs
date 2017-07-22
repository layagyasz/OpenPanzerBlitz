using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class ConvoyOrderDeployOrder : DeployOrder
	{
		public readonly ConvoyDeployment Deployment;
		public readonly IEnumerable<Unit> ConvoyOrder;

		public override Army Army
		{
			get
			{
				return Deployment.Army;
			}
		}

		public ConvoyOrderDeployOrder(ConvoyDeployment Deployment, IEnumerable<Unit> ConvoyOrder)
		{
			this.Deployment = Deployment;
			this.ConvoyOrder = ConvoyOrder;
		}

		public override NoDeployReason Validate()
		{
			return Deployment.Validate(ConvoyOrder);
		}

		public override bool Execute(Random Random)
		{
			if (Validate() == NoDeployReason.NONE)
			{
				Deployment.SetConvoyOrder(ConvoyOrder);
				return true;
			}
			return false;
		}
	}
}
