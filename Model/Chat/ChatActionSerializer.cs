using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ChatActionSerializer : SerializableAdapter
	{
		public static readonly ChatActionSerializer Instance = new ChatActionSerializer();

		ChatActionSerializer()
			: base(new Tuple<Type, Func<SerializationInputStream, Serializable>>[]
		{
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(AddChatMessageAction), i => new AddChatMessageAction(i))
		})
		{ }
	}
}
