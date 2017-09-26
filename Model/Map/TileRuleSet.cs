using System;

namespace PanzerBlitz
{
	public class TileRuleSet
	{
		public static readonly TileRuleSet SUMMER_STEPPE = new TileRuleSet(
			new TileComponentRules[]
			{
				TileComponentRules.BASE_CLEAR,
				TileComponentRules.BASE_SWAMP,
				TileComponentRules.BASE_SLOPE
			},
			new TileComponentRules[]
			{
				null,
				TileComponentRules.EDGE_TOWN,
				TileComponentRules.EDGE_FOREST,
				TileComponentRules.EDGE_SLOPE,
				TileComponentRules.EDGE_WATER
			},
			new TileComponentRules[]
			{
				null,
				TileComponentRules.PATH_ROAD,
				TileComponentRules.PATH_STREAM,
				TileComponentRules.PATH_STREAM_FORD
			}
		);

		TileComponentRules[] _BaseRules;
		TileComponentRules[] _EdgeRules;
		TileComponentRules[] _PathOverlayRules;

		public TileRuleSet(
			TileComponentRules[] BaseRules, TileComponentRules[] EdgeRules, TileComponentRules[] PathOverlayRules)
		{
			_BaseRules = BaseRules;
			_EdgeRules = EdgeRules;
			_PathOverlayRules = PathOverlayRules;
		}

		public TileComponentRules GetRules(TileBase Type)
		{
			return _BaseRules[(int)Type];
		}

		public TileComponentRules GetRules(TileEdge Type)
		{
			return _EdgeRules[(int)Type];
		}

		public TileComponentRules GetRules(TilePathOverlay Type)
		{
			return _PathOverlayRules[(int)Type];
		}
	}
}
