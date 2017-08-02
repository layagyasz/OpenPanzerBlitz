using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class OneOfZoneDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { DISPLAY_NAME, MATCHER }

		public readonly List<Matcher> Matchers;
		string _DisplayName;

		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
		}

		public OneOfZoneDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_DisplayName = (string)attributes[(int)Attribute.DISPLAY_NAME];
			Matchers = (List<Matcher>)attributes[(int)Attribute.MATCHER];
		}

		public Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
		{
			return new OneOfZoneDeployment(Army, Units, this, IdGenerator);
		}
	}
}
