using System;
namespace PanzerBlitz
{
	public class MatchContext : ProgramStateContext
	{
		public readonly Scenario Scenario;

		public MatchContext(Scenario Scenario)
		{
			this.Scenario = Scenario;
		}
	}
}
