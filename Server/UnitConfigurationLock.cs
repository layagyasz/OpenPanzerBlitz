using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfigurationLock
	{
		enum Attribute { UNIT_CONFIGURATIONS, RARITY }

		public readonly string UniqueId;
		public readonly List<UnitConfigurationLink> UnitConfigurations;
		public readonly float Rarity;

		public UnitConfigurationLock(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueId = Block.Name;
			UnitConfigurations = ((List<string>)attributes[(int)Attribute.UNIT_CONFIGURATIONS])
				.Select(i => GameData.UnitConfigurationLinks[i]).ToList();
			Rarity = (float)attributes[(int)Attribute.RARITY];
		}

		public double GetWeight()
		{
			return Math.Pow(2, -Rarity);
		}

		public double GetValue()
		{
			return Math.Pow(2, Rarity);
		}

		public override string ToString()
		{
			return string.Format("[UnitConfigurationLock: UniqueId={0}]", UniqueId);
		}
	}
}
