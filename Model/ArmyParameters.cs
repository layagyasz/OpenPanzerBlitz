using System;
namespace PanzerBlitz
{
	public class ArmyParameters
	{
		public readonly Faction Faction;
		public readonly int Points;
		public readonly ScenarioParameters Parameters;

		public ArmyParameters(Faction Faction, int Points, ScenarioParameters Parameters)
		{
			this.Faction = Faction;
			this.Points = Points;
			this.Parameters = Parameters;
		}

		public bool Matches(UnitConfigurationLink Link)
		{
			return (Link.Faction == null || Faction == Link.Faction) && Parameters.Matches(Link);
		}
	}
}
