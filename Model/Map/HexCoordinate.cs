using System;
namespace PanzerBlitz
{
	public class HexCoordinate
	{
		public readonly int X;
		public readonly int Y;
		public readonly int Z;

		public HexCoordinate(int X, int Y, int Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}

		public HexCoordinate(Coordinate From)
		{
			X = From.X - (From.Y + 1) / 2;
			Y = -(X + From.Y);
			Z = From.Y;
		}

		public int Distance(HexCoordinate To)
		{
			return Math.Max(Math.Abs(X - To.X), Math.Max(Math.Abs(Y - To.Y), Math.Abs(Z - To.Z)));
		}
	}
}
