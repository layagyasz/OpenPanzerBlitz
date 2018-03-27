using System;
using System.Collections.Generic;

using Cardamom.Serialization;

using Cence;

namespace PanzerBlitz
{
	public class FeatureGenerator : Serializable
	{
		enum Attribute { GENERATOR, THRESHOLD_GENERATOR }

		public readonly FunctionFactory Generator;
		public readonly FunctionFactory ThresholdGenerator;

		public FeatureGenerator(FunctionFactory Generator, FunctionFactory ThresholdGenerator)
		{
			this.Generator = Generator;
			this.ThresholdGenerator = ThresholdGenerator;
		}

		public FeatureGenerator(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Generator = (FunctionFactory)attributes[(int)Attribute.GENERATOR];
			ThresholdGenerator = (FunctionFactory)attributes[(int)Attribute.THRESHOLD_GENERATOR];
		}

		public FeatureGenerator(SerializationInputStream Stream)
			: this(
				Stream.ReadObject(FunctionFactory.Read, false, true),
				Stream.ReadObject(FunctionFactory.Read, false, true))
		{ }

		public Func<double, double, bool> GetFeatureGenerator(
			Random Random, Dictionary<FunctionFactory, Func<double, double, double>> Cache)
		{
			var g = Generator.Get(Random, Cache);
			var t = ThresholdGenerator.Get(Random, Cache);
			return (x, y) => g(x, y) > t(x, y);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Generator, false, true);
			Stream.Write(ThresholdGenerator, false, true);
		}
	}
}
