using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface ScenarioBuilderAction : Serializable
	{
		bool Apply(ScenarioBuilder ScenarioBuilder);
	}
}
