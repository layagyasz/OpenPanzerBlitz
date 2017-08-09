using System;
using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class DoTurnRequest : RPCRequest
	{
		public readonly TurnInfo TurnInfo;

		public DoTurnRequest(TurnInfo TurnInfo)
		{
			this.TurnInfo = TurnInfo;
		}

		public DoTurnRequest(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream)
		{
			TurnInfo = new TurnInfo(Stream, Objects);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(TurnInfo);
		}
	}
}
