using System;
using System.Collections.Generic;
using Cardamom.Planar;
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

		public ZoneDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_DisplayName = (string)attributes[(int)Attribute.DISPLAY_NAME];
			Matcher = (Matcher)attributes[(int)Attribute.MATCHER];
		}

		public Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
		{
			return new ZoneDeployment(Army, Units, this, IdGenerator);
		}
	}
}
