using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class OneOfZoneDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { UNIT_GROUP, MATCHER }

		public UnitGroup UnitGroup { get; }
		public readonly List<Matcher<Tile>> Matchers;

		public OneOfZoneDeploymentConfiguration(UnitGroup UnitGroup, IEnumerable<Matcher<Tile>> Matchers)
		{
			this.UnitGroup = UnitGroup;
			this.Matchers = Matchers.ToList();
		}

		public OneOfZoneDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitGroup = (UnitGroup)attributes[(int)Attribute.UNIT_GROUP];
			Matchers = (List<Matcher<Tile>>)attributes[(int)Attribute.MATCHER];
		}

		public OneOfZoneDeploymentConfiguration(SerializationInputStream Stream)
			: this(
				new UnitGroup(Stream),
				Stream.ReadEnumerable(i => (Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream)))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitGroup);
			Stream.Write(Matchers, i => MatcherSerializer.Instance.Serialize(i, Stream));
		}

		public Deployment GenerateDeployment(Army Army, IdGenerator IdGenerator)
		{
			return new OneOfZoneDeployment(Army, UnitGroup.GenerateUnits(Army, IdGenerator), this, IdGenerator);
		}
	}
}
