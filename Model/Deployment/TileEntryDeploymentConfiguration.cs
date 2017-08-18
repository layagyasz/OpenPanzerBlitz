using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileEntryDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { DISPLAY_NAME, MATCHER, IS_STRICT_CONVOY }

		public readonly bool IsStrictConvoy;
		public readonly Matcher Matcher;

		string _DisplayName;

		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
		}

		public TileEntryDeploymentConfiguration(string DisplayName, bool IsStrictConvoy, Matcher Matcher)
		{
			_DisplayName = DisplayName;
			this.IsStrictConvoy = IsStrictConvoy;
			this.Matcher = Matcher;
		}

		public TileEntryDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			_DisplayName = (string)attributes[(int)Attribute.DISPLAY_NAME];
			IsStrictConvoy = Parse.DefaultIfNull(attributes[(int)Attribute.IS_STRICT_CONVOY], false);

			Matcher m = (Matcher)attributes[(int)Attribute.MATCHER];
			Matcher edge = new TileOnEdge(Direction.ANY);

			if (m == null) Matcher = edge;
			else Matcher = new CompositeMatcher(new Matcher[] { edge, m }, CompositeMatcher.AND);
		}

		public TileEntryDeploymentConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadString(), Stream.ReadBoolean(), MatcherSerializer.Deserialize(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_DisplayName);
			Stream.Write(IsStrictConvoy);
			MatcherSerializer.Serialize(Matcher, Stream);
		}

		public Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
		{
			return new TileEntryDeployment(Army, Units, this, IdGenerator);
		}
	}
}
