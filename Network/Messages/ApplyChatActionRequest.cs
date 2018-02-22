using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ApplyChatActionRequest : RPCRequest
	{
		public readonly ChatAction Action;

		public ApplyChatActionRequest(ChatAction Action)
		{
			this.Action = Action;
		}

		public ApplyChatActionRequest(SerializationInputStream Stream)
			: base(Stream)
		{
			Action = (ChatAction)ChatActionSerializer.Instance.Deserialize(Stream);
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			ChatActionSerializer.Instance.Serialize(Action, Stream);
		}
	}
}
