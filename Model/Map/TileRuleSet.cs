using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileRuleSet
	{
		enum Attribute { BASES, EDGES, PATHS };

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

		public TileRuleSet(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_BaseRules = new TileComponentRules[Enum.GetValues(typeof(TileBase)).Length];
			_EdgeRules = new TileComponentRules[Enum.GetValues(typeof(TileEdge)).Length];
			_PathOverlayRules = new TileComponentRules[Enum.GetValues(typeof(TilePathOverlay)).Length];

			Func<string, TileBase> baseParser = Parse.EnumParser<TileBase>(typeof(TileBase));
			Func<string, TileEdge> edgeParser = Parse.EnumParser<TileEdge>(typeof(TileEdge));
			Func<string, TilePathOverlay> pathParser = Parse.EnumParser<TilePathOverlay>(typeof(TilePathOverlay));

			foreach (var p in (Dictionary<string, TileComponentRules>)attributes[(int)Attribute.BASES])
				_BaseRules[(int)baseParser(p.Key)] = p.Value;
			foreach (var p in (Dictionary<string, TileComponentRules>)attributes[(int)Attribute.EDGES])
				_EdgeRules[(int)edgeParser(p.Key)] = p.Value;
			foreach (var p in (Dictionary<string, TileComponentRules>)attributes[(int)Attribute.PATHS])
				_PathOverlayRules[(int)pathParser(p.Key)] = p.Value;
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
