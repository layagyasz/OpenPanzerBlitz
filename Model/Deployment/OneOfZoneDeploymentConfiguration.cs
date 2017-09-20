﻿using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class OneOfZoneDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { DISPLAY_NAME, MATCHER }

		public readonly List<Matcher<Tile>> Matchers;
		string _DisplayName;

		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
		}

		public OneOfZoneDeploymentConfiguration(string DisplayName, IEnumerable<Matcher<Tile>> Matchers)
		{
			_DisplayName = DisplayName;
			this.Matchers = Matchers.ToList();
		}

		public OneOfZoneDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_DisplayName = (string)attributes[(int)Attribute.DISPLAY_NAME];
			Matchers = (List<Matcher<Tile>>)attributes[(int)Attribute.MATCHER];
		}

		public OneOfZoneDeploymentConfiguration(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				Stream.ReadEnumerable(i => (Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream)))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_DisplayName);
			Stream.Write(Matchers, i => MatcherSerializer.Instance.Serialize(i, Stream));
		}

		public Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
		{
			return new OneOfZoneDeployment(Army, Units, this, IdGenerator);
		}
	}
}
