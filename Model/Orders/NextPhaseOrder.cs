using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class NextPhaseOrder : Order
	{
		public Army Army
		{
			get
			{
				return null;
			}
		}

		public NextPhaseOrder() { }

		public NextPhaseOrder(SerializationInputStream Stream, List<GameObject> Objects) { }

		public void Serialize(SerializationOutputStream Stream) { }

		public bool Validate()
		{
			return true;
		}

		public bool Execute(Random Random)
		{
			return true;
		}
	}
}
