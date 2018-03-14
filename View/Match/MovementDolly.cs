using System;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class MovementDolly
	{
		readonly CardinalSpline _Spline;
		readonly double _SpeedRecipricol;
		readonly Vector2f _End;

		double _Traveled;

		public bool Done
		{
			get
			{
				return Math.Abs(_Traveled - 1) < double.Epsilon || _Traveled > 1;
			}
		}

		public MovementDolly(UnitView UnitView, Path<Tile> Path, Unit Carrier)
		{
			_End = Path.Destination.Center;
			if (Path.Distance > 0)
			{
				_Spline = new CardinalSpline();
				foreach (Tile t in Path.Nodes.Where((x, i) => i % 2 == 0 || i == Path.Nodes.Count() - 1))
					_Spline.Points.Add(t.Center);

				bool unlimitedMove = Carrier == null
					? UnitView.Unit.Configuration.HasUnlimitedMovement()
				  	: Carrier.Configuration.HasUnlimitedMovement();
				if (unlimitedMove) _SpeedRecipricol = .0002;
				else
				{
					float move = Carrier == null
						? UnitView.Unit.Configuration.Movement
						: Carrier.Configuration.Movement;
					_SpeedRecipricol = .0002 * move / Path.Distance;
				}
			}
			else _Traveled = 1;
		}

		public Vector2f GetPoint(int DeltaT)
		{
			_Traveled += _SpeedRecipricol * DeltaT;
			if (_Traveled > 1) _Traveled = 1;

			if (_Traveled < 1) return _Spline.GetPoint(_Traveled);
			return _End;
		}
	}
}
