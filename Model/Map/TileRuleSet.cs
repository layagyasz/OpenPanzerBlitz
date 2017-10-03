using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileRuleSet
	{
		enum Attribute { BASES, EDGES, PATHS };

		public readonly string UniqueKey;

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

			UniqueKey = Block.Name;

			_BaseRules = Parse.KeyByEnum<TileBase, TileComponentRules>(
				(Dictionary<string, TileComponentRules>)attributes[(int)Attribute.BASES]);
			_EdgeRules = Parse.KeyByEnum<TileEdge, TileComponentRules>(
				(Dictionary<string, TileComponentRules>)attributes[(int)Attribute.EDGES]);
			_PathOverlayRules = Parse.KeyByEnum<TilePathOverlay, TileComponentRules>(
				(Dictionary<string, TileComponentRules>)attributes[(int)Attribute.PATHS]);
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
