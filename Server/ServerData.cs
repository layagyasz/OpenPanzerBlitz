using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ServerData
	{
		enum Attribute { UNIT_CONFIGURATION_LOCKS };

		public readonly Dictionary<string, UnitConfigurationLock> UnitConfigurationLocks;

		public ServerData(string Path)
		{
			var block = new ParseBlock(new ParseBlock[]
			{
				new ParseBlock(
					"unit-configuration-lock<>",
					"unit-configuration-locks",
					Directory.EnumerateFiles(Path + "/UnitConfigurationLocks", "*", SearchOption.AllDirectories)
						.SelectMany(i => ParseBlock.FromFile(i).Break()))
			});

			block.AddParser<UnitConfigurationLock>();

			var attributes = block.BreakToAttributes<object>(typeof(Attribute), true);
			UnitConfigurationLocks =
				(Dictionary<string, UnitConfigurationLock>)attributes[(int)Attribute.UNIT_CONFIGURATION_LOCKS];
		}
	}
}
