using System;
namespace PanzerBlitz
{
	public class HexCoordinate
	{
		public static HexCoordinate Interpolate(HexCoordinate c1, HexCoordinate c2, double t, double Dither = 0)
		{
			return Round(c1.X * (1 - t) + c2.X * t, c1.Y * (1 - t) + c2.Y * t, c1.Z * (1 - t) + c2.Z * t, Dither);
		}

		public static HexCoordinate Round(double X, double Y, double Z, double Dither = 0)
		{
			var x = (int)Math.Floor(X + .5 + Dither);
			var y = (int)Math.Floor(Y + .5 + Dither);
			var z = (int)Math.Floor(Z + .5 + Dither);

			var dX = Math.Abs(X - x);
			var dY = Math.Abs(Y - y);
			var dZ = Math.Abs(Z - z);

			if (dX > dY && dX > dZ) x = -(y + z);
			else if (dY > dZ) y = -(x + z);
			else z = -(x + y);

			return new HexCoordinate(x, y, z);
		}

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

		public Coordinate ToCoordinate()
		{
			return new Coordinate(this);
		}

		public int Distance(HexCoordinate To)
		{
			return Math.Max(Math.Abs(X - To.X), Math.Max(Math.Abs(Y - To.Y), Math.Abs(Z - To.Z)));
		}

		public override string ToString()
		{
			return string.Format("[HexCoordinate: X={0}, Y={1}, Z={2}]", X, Y, Z);
		}
	}
}
