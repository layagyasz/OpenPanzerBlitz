using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class SetScenarioParametersAction : ScenarioBuilderAction
	{
		public readonly ScenarioParameters Parameters;

		public SetScenarioParametersAction(ScenarioParameters Parameters)
		{
			this.Parameters = Parameters;
		}

		public bool Apply(ScenarioBuilder ScenarioBuilder)
		{
			return ScenarioBuilder.SetParameters(Parameters);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Parameters);
		}
	}
}
