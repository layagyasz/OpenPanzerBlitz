﻿using System;

using Cardamom.Network.Responses;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class NonGameMessageSerializer : SerializableAdapter
	{
		public NonGameMessageSerializer()
			: base(new Tuple<Type, Func<SerializationInputStream, Serializable>>[]
		{
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(ApplyLobbyActionRequest), i => new ApplyLobbyActionRequest(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(BooleanResponse), i => new BooleanResponse(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(GetLobbyRequest), i => new GetLobbyRequest(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(GetLobbyResponse), i => new GetLobbyResponse(i))
		})
		{ }
	}
}
