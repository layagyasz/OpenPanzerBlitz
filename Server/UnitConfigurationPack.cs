﻿using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class UnitConfigurationPack : Serializable
	{
		public readonly int Id;
		public readonly string Name;
		public readonly int Number;
		public readonly long Cost;
		public readonly double ExpectedValue;
		public readonly WeightedVector<UnitConfigurationLock> UnitConfigurationLocks;

		public UnitConfigurationPack(
			int Id, string Name, int Number, IEnumerable<UnitConfigurationLock> UnitConfigurationLocks)
		{
			this.Id = Id;
			this.Name = Name;
			this.Number = Number;
			this.UnitConfigurationLocks = new WeightedVector<UnitConfigurationLock>();
			foreach (var ucl in UnitConfigurationLocks) this.UnitConfigurationLocks.Add(ucl.GetWeight(), ucl);

			ExpectedValue = Number * HarmonicAverage(UnitConfigurationLocks.Select(i => i.GetValue()));
			Cost = RoundCost(
				.005
				* Multiplier(this.UnitConfigurationLocks.Length)
				* ExpectedValue,
				5);
		}

		public UnitConfigurationPack(SerializationInputStream Stream)
			: this(
				Stream.ReadInt32(),
				Stream.ReadString(),
				Stream.ReadInt32(),
				Stream.ReadEnumerable(i => new UnitConfigurationLock(i)))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Id);
			Stream.Write(Name);
			Stream.Write(Number);
			Stream.Write(UnitConfigurationLocks);
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
				"[UnitConfigurationPack: " + "" +
				"Id={0}, Name={1}, Number={2}, Cost={3}, ExpectedValue={4} UnitConfigurationLocks={5}]",
				Id,
				Name,
				Number,
				Cost,
				ExpectedValue,
				UnitConfigurationLocks.Length);
		}
	}
}
