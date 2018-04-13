using System;
using System.Collections.Generic;
using System.Linq;

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
				Stream.ReadEnumerable(i => (Unit)Objects[Stream.ReadInt32()]).ToList())
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Deployment.Id);
			Stream.Write(ConvoyOrder, i => Stream.Write(i.Id));
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.DEPLOYMENT;
		}

		public override OrderInvalidReason Validate()
		{
			return Deployment.Validate(ConvoyOrder);
		}

		public override OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Deployment.SetConvoyOrder(ConvoyOrder);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}

		public override string ToString()
		{
			return string.Format(
				"[ConvoyOrderDeployOrder: ConvoyOrder=`{0}`]",
				string.Join(", ", ConvoyOrder.Select(i => i.ToString())));
		}
	}
}
