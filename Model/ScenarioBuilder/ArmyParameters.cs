﻿using System;
namespace PanzerBlitz
{
	public class ArmyParameters
	{
		public Faction Faction { get; private set; }
		public uint Points { get; private set; }
		public byte Team { get; private set; }
		public ScenarioParameters Parameters { get; private set; }

		public ArmyParameters(Faction Faction, uint Points, byte Team, ScenarioParameters Parameters)
		{
			this.Faction = Faction;
			this.Points = Points;
			this.Team = Team;
			this.Parameters = Parameters;
		}

		public void Copy(ArmyParameters Copy)
		{
			Faction = Copy.Faction;
			Points = Copy.Points;
			Team = Copy.Team;
			Parameters = Copy.Parameters;
		}

		public bool Matches(UnitConfigurationLink Link)
		{
			return (Link.Faction == null || Faction == Link.Faction) && Parameters.Matches(Link);
		}

		public override string ToString()
		{
			return string.Format(
				"[ArmyParameters: Faction={0}, Points={1}, Team={2}, Parameters={3}]",
				Faction,
				Points,
				Team,
				Parameters);
		}
	}
}
