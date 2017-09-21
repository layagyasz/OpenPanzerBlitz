using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ZoneDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { UNIT_GROUP, MATCHER }

		public UnitGroup UnitGroup { get; }
		public readonly Matcher<Tile> Matcher;

		public ZoneDeploymentConfiguration(UnitGroup UnitGroup, Matcher<Tile> Matcher)
		{
			this.UnitGroup = UnitGroup;
			this.Matcher = Matcher;
		}

		public ZoneDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitGroup = (UnitGroup)attributes[(int)Attribute.UNIT_GROUP];
			Matcher = Parse.DefaultIfNull<Matcher<Tile>>(attributes[(int)Attribute.MATCHER], new EmptyMatcher<Tile>());
		}

		public ZoneDeploymentConfiguration(SerializationInputStream Stream)
			: this(new UnitGroup(Stream), (Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitGroup);
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public Deployment GenerateDeployment(Army Army, IdGenerator IdGenerator)
		{
			return new ZoneDeployment(Army, UnitGroup.GenerateUnits(Army, IdGenerator), this, IdGenerator);
		}
	}
}
