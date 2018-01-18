using System;
namespace PanzerBlitz
{
	public class ArmyParameters
	{
		public Faction Faction { get; private set; }
		public uint Points { get; private set; }
		public ScenarioParameters Parameters { get; private set; }

		public ArmyParameters(Faction Faction, uint Points, ScenarioParameters Parameters)
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
