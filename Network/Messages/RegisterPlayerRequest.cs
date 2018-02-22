using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class RegisterPlayerRequest : RPCRequest
	{
		public readonly string Username;
		public readonly string Password;

		public RegisterPlayerRequest(string Username, string Password)
		{
			this.Username = Username;
			this.Password = Password;
		}

		public RegisterPlayerRequest(SerializationInputStream Stream)
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