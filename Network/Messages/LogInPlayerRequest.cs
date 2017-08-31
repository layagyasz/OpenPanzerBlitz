using System;

using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class LogInPlayerRequest : RPCRequest
	{
		public readonly string Username;
		public readonly string Password;

		public LogInPlayerRequest(string Username, string Password)
		{
			this.Username = Username;
			this.Password = Password;
		}

		public LogInPlayerRequest(SerializationInputStream Stream)
			: base(Stream)
		{
			Username = Stream.ReadString();
			Password = Stream.ReadString();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(Username);
			Stream.Write(Password);
		}
	}
}
