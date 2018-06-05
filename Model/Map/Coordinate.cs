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

		public Coordinate(HexCoordinate From)
		{
			X = From.X + (From.Z + 1) / 2;
			Y = From.Z;
		}

		public Coordinate(SerializationInputStream Stream)
		{
			X = Stream.ReadInt32();
			Y = Stream.ReadInt32();
		}

		public Coordinate(ParseBlock Block)
		{
			var attributes = Block.String.Split(',');
			X = Convert.ToInt32(attributes[0]);
			Y = Convert.ToInt32(attributes[1]);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(X);
			Stream.Write(Y);
		}

		public HexCoordinate ToHexCoordinate()
		{
			return new HexCoordinate(this);
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj is Coordinate)
			{
				var o = (Coordinate)obj;
				return X == o.X && Y == o.Y;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public static bool operator ==(Coordinate c1, Coordinate c2)
		{
			if ((object)c1 == null) return (object)c2 == null;
			return c1.Equals(c2);
		}

		public static bool operator !=(Coordinate c1, Coordinate c2)
		{
			return !(c1 == c2);
		}

		public override string ToString()
		{
			return string.Format("[Coordinate: X={0}, Y={1}]", X, Y);
		}
	}
}
