using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { DISPLAY_NAME, COORDINATE }

		public readonly Coordinate Coordinate;
		string _DisplayName;

		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
		}

		public TileDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_DisplayName = (string)attributes[(int)Attribute.DISPLAY_NAME];
			Coordinate = (Coordinate)attributes[(int)Attribute.COORDINATE];
		}

		public Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units)
		{
			return new TileDeployment(Army, Units, this);
		}	}
}
