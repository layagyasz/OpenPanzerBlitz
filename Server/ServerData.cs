using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class ServerData
	{
		public readonly List<UnitConfigurationPack> UnitConfigurationPacks;

		public ServerData(Dictionary<string, UnitConfigurationLock> UnitConfigurationLocks)
		{
			UnitConfigurationPacks =
				new UnitConfigurationPackGenerator(GameData.Factions.Values)
					.Generate(UnitConfigurationLocks.Values).ToList();
		}
	}
}
