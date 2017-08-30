using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ServerData
	{
		enum Attribute { UNIT_CONFIGURATION_LOCKS, UNIT_CONFIGURATION_PACKS };

		public readonly Dictionary<string, UnitConfigurationLock> UnitConfigurationLocks;
		public readonly Dictionary<string, UnitConfigurationPack> UnitConfigurationPacks;

		public ServerData(string Path)
		{
			ParseBlock block = new ParseBlock(new ParseBlock[]
			{
				new ParseBlock(
					"unit-configuration-lock<>",
					"unit-configuration-locks",
					Directory.EnumerateFiles(Path + "/UnitConfigurationLocks", "*", SearchOption.AllDirectories)
						.SelectMany(i => ParseBlock.FromFile(i).Break())),
				ParseBlock.FromFile(Path + "/UnitConfigurationPacks.blk")
			});

			IdGenerator idGenerator = new IdGenerator();
			block.AddParser<TagMatcher>("tag-matcher", i => new TagMatcher(i));
			block.AddParser<UnitConfigurationLock>(
				"unit-configuration-lock",
				i => new UnitConfigurationLock(i, idGenerator));
			block.AddParser<UnitConfigurationPack>(
				"unit-configuration-pack", i => new UnitConfigurationPack(i, idGenerator));

			object[] attributes = block.BreakToAttributes<object>(typeof(Attribute), true);
			UnitConfigurationLocks =
				(Dictionary<string, UnitConfigurationLock>)attributes[(int)Attribute.UNIT_CONFIGURATION_LOCKS];
			UnitConfigurationPacks =
				(Dictionary<string, UnitConfigurationPack>)attributes[(int)Attribute.UNIT_CONFIGURATION_PACKS];
		}
	}
}
