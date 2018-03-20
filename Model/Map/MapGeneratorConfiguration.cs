using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities.Markov;

namespace PanzerBlitz
{
	public class MapGeneratorConfiguration : Serializable
	{
		enum Attribute
		{
			NAME_GENERATOR,
			FUNCTIONS, // Not read by class.  Used by parser to share function factories.
			ELEVATION_GENERATOR,
			WATER_GENERATOR,
			SWAMP_GENERATOR,
			FOREST_GENERATOR,
			TOWN_GENERATOR
		}

		public readonly string UniqueKey;
		public readonly MarkovGenerator<char> NameGenerator;
		public readonly FeatureGenerator ElevationGenerator;
		public readonly FeatureGenerator WaterGenerator;
		public readonly FeatureGenerator SwampGenerator;
		public readonly FeatureGenerator ForestGenerator;
		public readonly FeatureGenerator TownGenerator;

		public MapGeneratorConfiguration(
			string UniqueKey,
			MarkovGenerator<char> NameGenerator,
			FeatureGenerator ElevationGenerator,
			FeatureGenerator WaterGenerator,
			FeatureGenerator SwampGenerator,
			FeatureGenerator ForestGenerator,
			FeatureGenerator TownGenerator)
		{
			this.UniqueKey = UniqueKey;
			this.NameGenerator = NameGenerator;
			this.ElevationGenerator = ElevationGenerator;
			this.WaterGenerator = WaterGenerator;
			this.SwampGenerator = SwampGenerator;
			this.ForestGenerator = ForestGenerator;
			this.TownGenerator = TownGenerator;
		}

		public MapGeneratorConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);

			UniqueKey = Block.Name;
			NameGenerator = (MarkovGenerator<char>)attributes[(int)Attribute.NAME_GENERATOR];
			ElevationGenerator = (FeatureGenerator)attributes[(int)Attribute.ELEVATION_GENERATOR];
			WaterGenerator = (FeatureGenerator)attributes[(int)Attribute.WATER_GENERATOR];
			SwampGenerator = (FeatureGenerator)attributes[(int)Attribute.SWAMP_GENERATOR];
			ForestGenerator = (FeatureGenerator)attributes[(int)Attribute.FOREST_GENERATOR];
			TownGenerator = (FeatureGenerator)attributes[(int)Attribute.TOWN_GENERATOR];
		}

		public MapGeneratorConfiguration(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				GameData.NameGenerators[Stream.ReadString()],
				new FeatureGenerator(Stream),
				new FeatureGenerator(Stream),
				new FeatureGenerator(Stream),
				new FeatureGenerator(Stream),
				new FeatureGenerator(Stream))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(GameData.NameGenerators.First(i => i.Value == NameGenerator).Key);
			Stream.Write(ElevationGenerator);
			Stream.Write(WaterGenerator);
			Stream.Write(SwampGenerator);
			Stream.Write(ForestGenerator);
			Stream.Write(TownGenerator);
		}
	}
}
