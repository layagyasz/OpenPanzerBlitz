using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public struct Coordinate : Serializable
	{
		public readonly int X;
		public readonly int Y;

		public Coordinate(int X, int Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public Coordinate(SerializationInputStream Stream)
		{
			X = Stream.ReadInt32();
			Y = Stream.ReadInt32();
		}

		public Coordinate(ParseBlock Block)
		{
			string[] attributes = Block.String.Split(',');
			X = Convert.ToInt32(attributes[0]);
			Y = Convert.ToInt32(attributes[1]);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(X);
			Stream.Write(Y);
		}
	}
}
