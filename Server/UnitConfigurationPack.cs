using System;
using System.Collections.Generic;

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
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Id = IdGenerator.GenerateId();
			Number = (int)attributes[(int)Attribute.NUMBER];
			UnitConfigurationLocks = new WeightedVector<UnitConfigurationLock>();
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
