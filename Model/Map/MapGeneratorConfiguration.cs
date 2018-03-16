using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities.Markov;

namespace PanzerBlitz
{
	public class MapGeneratorConfiguration : Serializable
	{
		enum Attribute { NAME_GENERATOR }

		public readonly string UniqueKey;
		public readonly MarkovGenerator<char> NameGenerator;

		public MapGeneratorConfiguration(string UniqueKey, MarkovGenerator<char> Language)
		{
			this.UniqueKey = UniqueKey;
			this.NameGenerator = Language;
		}

		public MapGeneratorConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			NameGenerator = (MarkovGenerator<char>)attributes[(int)Attribute.NAME_GENERATOR];
		}

		public MapGeneratorConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadString(), GameData.NameGenerators[Stream.ReadString()]) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(GameData.NameGenerators.First(i => i.Value == NameGenerator).Key);
		}
	}
}
