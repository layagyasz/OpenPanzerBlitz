using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileEntryDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { UNIT_GROUP, MATCHER, IS_STRICT_CONVOY }

		public UnitGroup UnitGroup { get; }
		public readonly bool IsStrictConvoy;
		public readonly Matcher<Tile> Matcher;

		public TileEntryDeploymentConfiguration(UnitGroup UnitGroup, bool IsStrictConvoy, Matcher<Tile> Matcher)
		{
			this.UnitGroup = UnitGroup;
			this.IsStrictConvoy = IsStrictConvoy;
			this.Matcher = Matcher;
		}

		public TileEntryDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitGroup = (UnitGroup)attributes[(int)Attribute.UNIT_GROUP];
			IsStrictConvoy = Parse.DefaultIfNull(attributes[(int)Attribute.IS_STRICT_CONVOY], false);

			Matcher<Tile> m = (Matcher<Tile>)attributes[(int)Attribute.MATCHER];
			Matcher<Tile> edge = new TileOnEdge(Direction.ANY);

			if (m == null) Matcher = edge;
			else Matcher = new CompositeMatcher<Tile>(new Matcher<Tile>[] { edge, m }, CompositeMatcher<Tile>.AND);
		}

		public TileEntryDeploymentConfiguration(SerializationInputStream Stream)
			: this(
				new UnitGroup(Stream),
				Stream.ReadBoolean(),
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitGroup);
			Stream.Write(IsStrictConvoy);
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public Deployment GenerateDeployment(Army Army, IdGenerator IdGenerator)
		{
			return new TileEntryDeployment(Army, UnitGroup.GenerateUnits(Army, IdGenerator), this, IdGenerator);
		}
	}
}
