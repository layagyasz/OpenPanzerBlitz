using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class UnitConfigurationPackGenerator
	{
		readonly List<Faction> _Factions;

		public UnitConfigurationPackGenerator(IEnumerable<Faction> Factions)
		{
			_Factions = Factions.ToList();
		}

		public IEnumerable<UnitConfigurationPack> Generate(IEnumerable<UnitConfigurationLock> UnitConfigurationLocks)
		{
			var idGenerator = new IdGenerator();
			foreach (Faction faction in _Factions.Concat(new Faction[] { null }))
			{
				foreach (UnitClass unitClass in Enum.GetValues(typeof(UnitClass)).Cast<UnitClass>())
				{
					var locks =
						UnitConfigurationLocks.Where(FactionFilter(faction)).Where(UnitClassFilter(unitClass)).ToList();
					if (locks.Count > 0)
					{
						yield return new UnitConfigurationPack(
							idGenerator.GenerateId(),
							PackName(faction, unitClass),
							9,
							locks);
					}
				}
			}
		}

		string PackName(Faction Faction, UnitClass UnitClass)
		{
			if (Faction == null && UnitClass == UnitClass.NONE) return "Random Pack";
			if (Faction == null) return string.Format("{0} Pack", ObjectDescriber.Describe(UnitClass));
			if (UnitClass == UnitClass.NONE) return string.Format("{0} Pack", Faction.Name);
			return string.Format("{0} {1} Pack", Faction.Name, ObjectDescriber.Describe(UnitClass));
		}

		Func<UnitConfigurationLock, bool> FactionFilter(Faction Faction)
		{
			if (Faction == null) return i => true;
			return i => i.UnitConfigurations.All(j => j.Faction == Faction);
		}

		Func<UnitConfigurationLock, bool> UnitClassFilter(UnitClass UnitClass)
		{
			if (UnitClass == UnitClass.NONE) return i => true;
			return i => i.UnitConfigurations.All(j => j.UnitConfiguration.UnitClass == UnitClass);
		}
	}
}
