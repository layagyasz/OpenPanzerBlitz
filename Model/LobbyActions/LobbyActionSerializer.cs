using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class LobbyActionSerializer : SerializableAdapter
	{
		public static readonly LobbyActionSerializer Instance = new LobbyActionSerializer();

		LobbyActionSerializer()
			: base(new Tuple<Type, Func<SerializationInputStream, Serializable>>[]
		{
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(AddPlayerAction), i => new AddPlayerAction(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(SetPlayerArmyAction), i => new SetPlayerArmyAction(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(SetPlayerReadyAction), i => new SetPlayerReadyAction(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(SetScenarioAction), i => new SetScenarioAction(i))
		})
		{ }
	}
}
