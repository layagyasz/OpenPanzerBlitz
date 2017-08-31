using System;

using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class LogInPlayerResponse : RPCResponse
	{
		public readonly Player Player;

		public LogInPlayerResponse(Player Player)
		{
			this.Player = Player;
		}

		public LogInPlayerResponse(SerializationInputStream Stream)
		: base(Stream)
		{
			Player = Stream.ReadNullOrObject(i => new Player(Stream));
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(Player, true);
		}
	}
}
