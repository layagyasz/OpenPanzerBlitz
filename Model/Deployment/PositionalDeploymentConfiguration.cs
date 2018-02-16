using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class PositionalDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { UNIT_GROUP, MATCHER }

		public UnitGroup UnitGroup { get; }
		public readonly Matcher<Tile> Matcher;

		public PositionalDeploymentConfiguration(UnitGroup UnitGroup, Matcher<Tile> Matcher)
		{
			this.UnitGroup = UnitGroup;
			this.Matcher = Matcher;
		}

		public PositionalDeploymentConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitGroup = (UnitGroup)attributes[(int)Attribute.UNIT_GROUP];
			Matcher = Parse.DefaultIfNull<Matcher<Tile>>(attributes[(int)Attribute.MATCHER], new EmptyMatcher<Tile>());
		}

		public PositionalDeploymentConfiguration(SerializationInputStream Stream)
			: this(new UnitGroup(Stream), (Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitGroup);
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public Deployment GenerateDeployment(Army Army, IdGenerator IdGenerator)
		{
			return new PositionalDeployment(Army, UnitGroup.GenerateUnits(Army, IdGenerator), this, IdGenerator);
		}
	}
}
