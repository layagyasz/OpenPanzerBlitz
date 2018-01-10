using System;
namespace PanzerBlitz
{
	public class ScenarioParameters
	{
		public readonly int Year;
		public readonly Front Front;
		public readonly Environment Environment;

		public ScenarioParameters(int Year, Front Front, Environment Environment)
		{
			this.Year = Year;
			this.Front = Front;
			this.Environment = Environment;
		}

		public bool Matches(UnitConfigurationLink Link)
		{
			if (Front != Front.ALL && Link.Front != Front.ALL && Front != Link.Front) return false;
			if (Link.IntroduceYear > 0 && Year < Link.IntroduceYear) return false;
			if (Link.ObsoleteYear > 0 && Year > Link.ObsoleteYear) return false;
			if (Link.Environments != null && !Link.Environments.Contains(Environment)) return false;
			return true;
		}
	}
}
