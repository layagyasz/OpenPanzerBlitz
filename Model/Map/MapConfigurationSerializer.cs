using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MapConfigurationSerializer : SerializableAdapter
	{
		public static readonly MapConfigurationSerializer Instance = new MapConfigurationSerializer();

		MapConfigurationSerializer()
			: base(
				typeof(BlankMapConfiguration),
				typeof(BoardCompositeMapConfiguration),
				typeof(RandomMapConfiguration),
				typeof(StaticMapConfiguration))
		{ }
	}
}
