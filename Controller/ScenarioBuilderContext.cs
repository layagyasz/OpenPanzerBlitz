using System;
namespace PanzerBlitz
{
	public class ScenarioBuilderContext : ProgramStateContext
	{
		public readonly ScenarioBuilder ScenarioBuilder;

		public ScenarioBuilderContext(ScenarioBuilder ScenarioBuilder)
		{
			this.ScenarioBuilder = ScenarioBuilder;
		}
	}
}
