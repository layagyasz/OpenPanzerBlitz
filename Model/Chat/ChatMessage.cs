using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ChatMessage : Serializable
	{
		public readonly Player Player;
		public readonly DateTime Time;
		public readonly string Message;

		public ChatMessage(Player Player, string Message)
		{
			this.Player = Player;
			this.Time = DateTime.Now;
			this.Message = Message;
		}

		public ChatMessage(SerializationInputStream Stream)
		{
			Player = new Player(Stream);
			Time = new DateTime(Stream.ReadInt64());
			Message = Stream.ReadString();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Player);
			Stream.Write(Time.Ticks);
			Stream.Write(Message);
		}
	}
}
