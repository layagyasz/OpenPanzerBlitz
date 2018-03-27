using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TerrainGeneratorConfiguration : Serializable
	{
		enum Attribute
		{
			FUNCTIONS, // Not read by class.  Used by parser to share function factories.
			ELEVATION_GENERATOR,
			WATER_GENERATOR,
			SWAMP_GENERATOR,
			FOREST_GENERATOR,
			TOWN_GENERATOR
		}

		public readonly string UniqueKey;
		public readonly FeatureGenerator ElevationGenerator;
		public readonly FeatureGenerator WaterGenerator;
		public readonly FeatureGenerator SwampGenerator;
		public readonly FeatureGenerator ForestGenerator;
		public readonly FeatureGenerator TownGenerator;

		public TerrainGeneratorConfiguration(
			string UniqueKey,
			FeatureGenerator ElevationGenerator,
			FeatureGenerator WaterGenerator,
			FeatureGenerator SwampGenerator,
			FeatureGenerator ForestGenerator,
			FeatureGenerator TownGenerator)
		{
			this.UniqueKey = UniqueKey;
			this.ElevationGenerator = ElevationGenerator;
			this.WaterGenerator = WaterGenerator;
			this.SwampGenerator = SwampGenerator;
			this.ForestGenerator = ForestGenerator;
			this.TownGenerator = TownGenerator;
		}

		public TerrainGeneratorConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);

			UniqueKey = Block.Name;
			ElevationGenerator = (FeatureGenerator)attributes[(int)Attribute.ELEVATION_GENERATOR];
			WaterGenerator = (FeatureGenerator)attributes[(int)Attribute.WATER_GENERATOR];
			SwampGenerator = (FeatureGenerator)attributes[(int)Attribute.SWAMP_GENERATOR];
			ForestGenerator = (FeatureGenerator)attributes[(int)Attribute.FOREST_GENERATOR];
			TownGenerator = (FeatureGenerator)attributes[(int)Attribute.TOWN_GENERATOR];
		}

		public TerrainGeneratorConfiguration(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				new FeatureGenerator(Stream),
				new FeatureGenerator(Stream),
				new FeatureGenerator(Stream),
				new FeatureGenerator(Stream),
				new FeatureGenerator(Stream))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(ElevationGenerator);
			Stream.Write(WaterGenerator);
			Stream.Write(SwampGenerator);
			Stream.Write(ForestGenerator);
			Stream.Write(TownGenerator);
		}
	}
}
