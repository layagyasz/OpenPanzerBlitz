using System;
namespace PanzerBlitz
{
	public class ArmyParameters
	{
		public readonly int Points;
		public readonly int Year;
		public readonly Front Front;

		public ArmyParameters(int Points, int Year, Front Front)
		{
			this.Points = Points;
			this.Year = Year;
			this.Front = Front;
		}

		public bool Matches(UnitConfigurationLink Link)
		{
			if (Front != Front.ALL && Link.Front != Front.ALL && Front != Link.Front) return false;
			if (Link.IntroduceYear > 0 && Year < Link.IntroduceYear) return false;
			if (Link.ObsoleteYear > 0 && Year > Link.ObsoleteYear) return false;
			return true;
		}
	}
}
