using Cardamom.Serialization;
using Cardamom.Utilities.Markov;

namespace PanzerBlitz
{
	public class MapGeneratorConfiguration : Serializable
	{
		enum Attribute { LANGUAGE_PATH }

		public readonly string UniqueKey;
		public readonly MarkovGenerator<char> Language;

		public MapGeneratorConfiguration(string UniqueKey, MarkovGenerator<char> Language)
		{
			this.UniqueKey = UniqueKey;
			this.Language = Language;
		}

		public MapGeneratorConfiguration(ParseBlock Block, string RootPath)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			Language =
				FileUtils.LoadLanguage(
					RootPath + "/LanguageGenerators/" + (string)attributes[(int)Attribute.LANGUAGE_PATH]);
		}

		public MapGeneratorConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadString(), new MarkovGenerator<char>(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(Language);
		}
	}
}
