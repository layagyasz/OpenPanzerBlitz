using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MatchSetting : Serializable
	{
		enum Attribute { ENVIRONMENT, MAP_GENERATOR }

		public readonly string UniqueKey;
		public readonly Environment Environment;
		public readonly MapGeneratorConfiguration MapGenerator;

		public MatchSetting(string UniqueKey, Environment Environment, MapGeneratorConfiguration MapGenerator)
		{
			this.UniqueKey = UniqueKey;
			this.Environment = Environment;
			this.MapGenerator = MapGenerator;
		}

		public MatchSetting(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);

			UniqueKey = Block.Name;
			Environment = (Environment)attributes[(int)Attribute.ENVIRONMENT];
			MapGenerator = (MapGeneratorConfiguration)attributes[(int)Attribute.MAP_GENERATOR];
		}

		public MatchSetting(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				GameData.Environments[Stream.ReadString()],
				new MapGeneratorConfiguration(Stream))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(Environment.UniqueKey);
			Stream.Write(MapGenerator);
		}

		public override string ToString()
		{
			return string.Format("[MatchSetting: UniqueKey={0}]", UniqueKey);
		}
	}
}
