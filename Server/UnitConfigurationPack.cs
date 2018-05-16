using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class UnitConfigurationPack
	{
		public readonly int Id;
		public readonly string Name;
		public readonly int Number;
		public readonly long Cost;
		public readonly WeightedVector<UnitConfigurationLock> UnitConfigurationLocks;

		public UnitConfigurationPack(
			IdGenerator IdGenerator, string Name, int Number, IEnumerable<UnitConfigurationLock> UnitConfigurationLocks)
		{
			Id = IdGenerator.GenerateId();
			this.Name = Name;
			this.Number = Number;
			this.UnitConfigurationLocks = new WeightedVector<UnitConfigurationLock>();
			foreach (var ucl in UnitConfigurationLocks) this.UnitConfigurationLocks.Add(ucl.GetWeight(), ucl);

			Cost = RoundCost(
				.002
				* Number
				* Multiplier(this.UnitConfigurationLocks.Length)
				* HarmonicAverage(UnitConfigurationLocks.Select(i => i.GetValue())),
				5);
		}

		public IEnumerable<UnitConfigurationLock> Open(Random Random)
		{
			for (int i = 0; i < Number; ++i)
			{
				yield return UnitConfigurationLocks[Random.NextDouble()];
			}
		}

		static double Multiplier(int Count)
		{
			if (Count > 1) return Math.Log(Count) / Math.Log(2);
			return 2;
		}

		static double HarmonicAverage(IEnumerable<double> Values)
		{
			return Values.Count() / Values.Sum(i => 1.0 / i);
		}

		static long RoundCost(double Cost, int Step)
		{
			return (long)(Step * Math.Round(Cost / Step));
		}

		public override string ToString()
		{
			return string.Format(
				"[UnitConfigurationPack: Id={0}, Name={1}, Number={2}, Cost={3}, UnitConfigurationLocks={4}]",
				Id,
				Name,
				Number,
				Cost,
				UnitConfigurationLocks.Length);
		}
	}
}
