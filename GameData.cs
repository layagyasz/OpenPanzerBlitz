﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Planar;
using Cardamom.Serialization;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class GameData
	{
		private enum Attribute { FACTIONS, UNIT_CONFIGURATIONS, SCENARIOS };

		public readonly Dictionary<string, Faction> Factions;
		public readonly Dictionary<string, UnitConfiguration> UnitConfigurations;
		public readonly List<Scenario> Scenarios;

		public GameData(string Path)
			: this(new ParseBlock(string.Join("\n", Directory.EnumerateFiles(Path).Select(i => File.ReadAllText(i)))))
		{ }

		public GameData(ParseBlock Block)
		{
			Block.AddParser<Color>("color", i => ClassLibrary.Instance.ParseColor(i.String), false);
			Block.AddParser<List<Color>>("color[]", i => ClassLibrary.Instance.ParseColors(i.String), false);
			Block.AddParser<Dictionary<string, Color>>("color<>", i => i.BreakToDictionary<Color>(), false);
			Block.AddParser<TileElevation>("tile-elevation", i => new TileElevation(i));
			Block.AddParser<TileWithin>("tile-within", i => new TileWithin(i));
			Block.AddParser<DistanceFromUnit>("distance-from-unit", i => new DistanceFromUnit(i));
			Block.AddParser<Polygon>("zone", i => new Polygon(i));
			Block.AddParser<Coordinate>("coordinate", i => new Coordinate(i));
			Block.AddParser<CompositeMatcher>("and", i => new CompositeMatcher(i, (j, k) => j && k));
			Block.AddParser<CompositeMatcher>("or", i => new CompositeMatcher(i, (j, k) => j || k));

			Block.AddParser<WeaponClass>("weapon-class", Parse.EnumParser<WeaponClass>(typeof(WeaponClass)));
			Block.AddParser<UnitClass>("unit-class", Parse.EnumParser<UnitClass>(typeof(UnitClass)));
			Block.AddParser<Faction>("faction", i => new Faction(i));
			Block.AddParser<UnitConfiguration>("unit-configuration", i => new UnitConfiguration(i));
			Block.AddParser<DeploymentConfiguration>(
				"tile-deployment-configuration", i => new TileDeploymentConfiguration(i));
			Block.AddParser<DeploymentConfiguration>(
				"zone-deployment-configuration", i => new ZoneDeploymentConfiguration(i));
			Block.AddParser<TileEntryDeployment>(
				"tile-entry-deployment-configuration", i => new TileEntryDeploymentConfiguration(i));
			Block.AddParser<ArmyConfiguration>("army-configuration", i => new ArmyConfiguration(i));
			Block.AddParser<MapConfiguration>("map-configuration", i => new MapConfiguration(i));
			Block.AddParser<Scenario>("scenario", i => new Scenario(i));

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			Factions = (Dictionary<string, Faction>)attributes[(int)Attribute.FACTIONS];
			UnitConfigurations = (Dictionary<string, UnitConfiguration>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
			Scenarios = (List<Scenario>)attributes[(int)Attribute.SCENARIOS];
		}
	}
}
