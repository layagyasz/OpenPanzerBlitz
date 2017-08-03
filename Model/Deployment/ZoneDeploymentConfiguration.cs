using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ZoneDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { DISPLAY_NAME, MATCHER }

		public readonly Matcher Matcher;
		string _DisplayName;

		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
		}

		public ZoneDeploymentConfiguration(string DisplayName, Matcher Matcher)
		{
			_DisplayName = DisplayName;
			this.Matcher = Matcher;
		}

		public ZoneDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_DisplayName = (string)attributes[(int)Attribute.DISPLAY_NAME];
			Matcher = (Matcher)attributes[(int)Attribute.MATCHER];
		}

		public ZoneDeploymentConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadString(), MatcherSerializer.Deserialize(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_DisplayName);
			MatcherSerializer.Serialize(Matcher, Stream);
		}

		public Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
		{
			return new ZoneDeployment(Army, Units, this, IdGenerator);
		}
	}
}
