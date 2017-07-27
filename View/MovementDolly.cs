using System;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class MovementDolly
	{
		CardinalSpline _Spline;
		double _Traveled;
		double _SpeedRecipricol;

		public bool Done
		{
			get
			{
				return Math.Abs(_Traveled - 1) < double.Epsilon || _Traveled > 1;
			}
		}

		public MovementDolly(UnitView UnitView, Path<Tile> Path)
		{
			_Spline = new CardinalSpline();
			foreach (Tile t in Path.Nodes.Where((x, i) => i % 2 == 0 || i == Path.Nodes.Count() - 1))
				_Spline.Points.Add(t.Center);

			float move = UnitView.Unit.Carrier == null
								 ? UnitView.Unit.Configuration.Movement
								 : UnitView.Unit.Carrier.Configuration.Movement;
			_SpeedRecipricol = .0002 * move / Path.Distance;
		}

		public Vector2f GetPoint(int DeltaT)
		{
			_Traveled += _SpeedRecipricol * DeltaT;
			if (_Traveled > 1) _Traveled = 1;
			return _Spline.GetPoint(_Traveled);
		}
	}
}
