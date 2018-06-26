using Cardamom.Serialization;
using Cardamom.Utilities.Markov;

namespace PanzerBlitz
{
	public class MapGeneratorConfiguration : Serializable
	{
		enum Attribute
		{
			NAME_GENERATOR,
			TERRAIN_GENERATOR
		}

		public readonly string UniqueKey;
		public readonly MarkovGenerator<char> NameGenerator;
		public readonly TerrainGeneratorConfiguration TerrainGenerator;

		public MapGeneratorConfiguration(
			string UniqueKey,
			MarkovGenerator<char> NameGenerator,
			TerrainGeneratorConfiguration TerrainGenerator)
		{
			this.UniqueKey = UniqueKey;
			this.NameGenerator = NameGenerator;
			this.TerrainGenerator = TerrainGenerator;
		}

		public MapGeneratorConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);

			UniqueKey = Block.Name;
			NameGenerator = (MarkovGenerator<char>)attributes[(int)Attribute.NAME_GENERATOR];
			TerrainGenerator = (TerrainGeneratorConfiguration)attributes[(int)Attribute.TERRAIN_GENERATOR];
		}

		public MapGeneratorConfiguration(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				Stream.ReadObject(i => new MarkovGenerator<char>(i), false, true),
				Stream.ReadObject(i => new TerrainGeneratorConfiguration(i), false, true))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(NameGenerator, false, true);
			Stream.Write(TerrainGenerator, false, true);
		}
	}
}
