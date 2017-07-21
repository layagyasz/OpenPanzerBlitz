using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileEntryDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { DISPLAY_NAME }

		string _DisplayName;

		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
		}

		public TileEntryDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			_DisplayName = (string)attributes[(int)Attribute.DISPLAY_NAME];
		}

		public Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units)
		{
			return new TileEntryDeployment(Army, Units, this);
		}
	}
}
