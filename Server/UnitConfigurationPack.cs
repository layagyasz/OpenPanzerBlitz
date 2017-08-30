using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class UnitConfigurationPack
	{
		static readonly int COST_SCALE = 100;
		static readonly int COST_STEP = 5;

		enum Attribute { NAME, NUMBER, TAG_MATCHER }

		public readonly int Id;
		public readonly int Number;
		public readonly int Cost;
		public readonly WeightedVector<UnitConfigurationLock> UnitConfigurationLocks;

		public UnitConfigurationPack(
			ParseBlock Block, IdGenerator IdGenerator)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Id = IdGenerator.GenerateId();
			Number = (int)attributes[(int)Attribute.NUMBER];
			UnitConfigurationLocks = new WeightedVector<UnitConfigurationLock>();

			float cost = 0;
			float totalCost = 0;
			Dictionary<string, UnitConfigurationLock> locks =
				Block.Get<Dictionary<string, UnitConfigurationLock>>("unit-configuration-locks");
			TagMatcher m = (TagMatcher)attributes[(int)Attribute.TAG_MATCHER];
			foreach (UnitConfigurationLock c in locks.Values)
			{
				float lockCost = c.UnitConfiguration.GetPointValue() / c.Rarity;
				if (m.Matches(c.Tags))
				{
					UnitConfigurationLocks.Add(c.Rarity, c);
					cost += lockCost;
				}
				totalCost += lockCost;
			}
			Cost = RoundCost(cost / this.UnitConfigurationLocks.Length, COST_SCALE, COST_STEP);
		}

		public IEnumerable<UnitConfigurationLock> Open(Random Random)
		{
			for (int i = 0; i < Number; ++i)
			{
				yield return UnitConfigurationLocks[Random.NextDouble()];
			}
		}

		static int RoundCost(float Cost, int Scale, int Step)
		{
			return Step * (int)(Cost * Scale / Step);
		}
	}
}
