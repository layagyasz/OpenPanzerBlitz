using System;
using System.Collections.Generic;

using Cardamom.Serialization;

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

		public ConvoyOrderDeployOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this(
				(ConvoyDeployment)Objects[Stream.ReadInt32()],
				Stream.ReadEnumerable(i => (Unit)Objects[Stream.ReadInt32()]))
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Deployment.Id);
			Stream.Write(ConvoyOrder, i => Stream.Write(i.Id));
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
