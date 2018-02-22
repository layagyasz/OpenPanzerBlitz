using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ScenarioBuilderActionSerializer : SerializableAdapter
	{
		public static readonly ScenarioBuilderActionSerializer Instance = new ScenarioBuilderActionSerializer();

		ScenarioBuilderActionSerializer()
			: base(typeof(ScenarioBuilderAction)) { }
	}
}
