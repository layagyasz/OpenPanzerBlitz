using System;

using Cardamom.Network.Responses;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class NonMatchMessageSerializer : SerializableAdapter
	{
		public NonMatchMessageSerializer()
			: base(new Tuple<Type, Func<SerializationInputStream, Serializable>>[]
		{
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(ApplyLobbyActionRequest), i => new ApplyLobbyActionRequest(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(BooleanResponse), i => new BooleanResponse(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(GetLobbyRequest), i => new GetLobbyRequest(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(GetLobbyResponse), i => new GetLobbyResponse(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(ApplyChatActionRequest), i => new ApplyChatActionRequest(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(LogInPlayerRequest), i => new LogInPlayerRequest(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(RegisterPlayerRequest), i => new RegisterPlayerRequest(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(LogInPlayerResponse), i => new LogInPlayerResponse(i))
		})
		{ }
	}
}
