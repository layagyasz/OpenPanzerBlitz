using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Planar;
using Cardamom.Serialization;

using SFML.Graphics;

namespace PanzerBlitz
{
	public static class GameData
	{
		enum Attribute { FACTIONS, UNIT_CONFIGURATIONS, UNIT_CONFIGURATION_LINKS, SCENARIOS };

		public static ushort OnlinePort = 1000;
		public static Player Player = new Player((int)DateTime.Now.Ticks, "Player " + DateTime.Now.Ticks.ToString(), true);
		public static Dictionary<string, Faction> Factions;
		public static Dictionary<string, UnitConfiguration> UnitConfigurations;
		public static List<UnitConfigurationLink> UnitConfigurationLinks;
		public static UnitConfiguration Wreckage;
		public static List<Scenario> Scenarios;

		public static void Load(string Path)
		{
			ParseBlock block = new ParseBlock(new ParseBlock[] {
				ParseBlock.FromFile(Path + "/Factions.blk"),
				new ParseBlock(
					"unit-configuration<>",
					"unit-configurations",
					Directory.EnumerateFiles(Path + "/UnitConfigurations", "*", SearchOption.AllDirectories)
						.SelectMany(i => ParseBlock.FromFile(i).Break())),
				new ParseBlock(
					"unit-configuration-link[]",
					"unit-configuration-links",
					Directory.EnumerateFiles(Path + "/UnitConfigurationLinks", "*", SearchOption.AllDirectories)
						   .SelectMany(i => ParseBlock.FromFile(i).Break())),
				new ParseBlock(
					"scenario[]",
					"scenarios",
					Directory.EnumerateFiles(Path + "/Scenarios", "*", SearchOption.AllDirectories)
						.Select(i => ParseBlock.FromFile(i)))
			});
			Load(block);
		}

		public static void Load(ParseBlock Block)
		{
			Block.AddParser<Color>("color", i => ClassLibrary.Instance.ParseColor(i.String), false);
			Block.AddParser<List<Color>>("color[]", i => ClassLibrary.Instance.ParseColors(i.String), false);
			Block.AddParser<Dictionary<string, Color>>("color<>", i => i.BreakToDictionary<Color>(), false);

			Block.AddParser<Polygon>("zone", i => new Polygon(i));
			Block.AddParser<Coordinate>("coordinate", i => new Coordinate(i));
			Block.AddParser<WeaponClass>("weapon-class", Parse.EnumParser<WeaponClass>(typeof(WeaponClass)));
			Block.AddParser<UnitClass>("unit-class", Parse.EnumParser<UnitClass>(typeof(UnitClass)));
			Block.AddParser<UnitStatus>("unit-status", Parse.EnumParser<UnitStatus>(typeof(UnitStatus)));
			Block.AddParser<Faction>("faction", i => new Faction(i));
			Block.AddParser<UnitConfiguration>("unit-configuration", i => new UnitConfiguration(i));
			Block.AddParser<UnitConfigurationLink>("unit-configuration-link", i => new UnitConfigurationLink(i));
			Block.AddParser<Direction>("direction", Parse.EnumParser<Direction>(typeof(Direction)));

			Block.AddParser<Matcher<Tile>>("tile-elevation", i => new TileElevation(i));
			Block.AddParser<Matcher<Tile>>("tile-within", i => new TileWithin(i));
			Block.AddParser<Matcher<Tile>>("tile-on-edge", i => new TileOnEdge(i));
			Block.AddParser<Matcher<Tile>>("tile-distance-from-unit", i => new TileDistanceFromUnit(i));
			Block.AddParser<Matcher<Tile>>(
				"tile-matches-all", i => new CompositeMatcher<Tile>(i, CompositeMatcher<Tile>.AND));
			Block.AddParser<Matcher<Tile>>(
				"tile-matches-any", i => new CompositeMatcher<Tile>(i, CompositeMatcher<Tile>.OR));

			Block.AddParser<Matcher<Unit>>("unit-has-evacuated", i => new UnitHasEvacuated(i));
			Block.AddParser<Matcher<Unit>>("unit-has-reconned", i => new UnitHasReconned(i));
			Block.AddParser<Matcher<Unit>>("unit-has-status", i => new UnitHasStatus(i));
			Block.AddParser<Matcher<Unit>>(
				"unit-matches-all", i => new CompositeMatcher<Unit>(i, CompositeMatcher<Unit>.AND));
			Block.AddParser<Matcher<Unit>>(
				"unit-matches-any", i => new CompositeMatcher<Unit>(i, CompositeMatcher<Unit>.OR));

			Block.AddParser<DeploymentConfiguration>(
				"tile-deployment-configuration", i => new TileDeploymentConfiguration(i));
			Block.AddParser<DeploymentConfiguration>(
				"zone-deployment-configuration", i => new ZoneDeploymentConfiguration(i));
			Block.AddParser<DeploymentConfiguration>(
				"tile-entry-deployment-configuration", i => new TileEntryDeploymentConfiguration(i));
			Block.AddParser<DeploymentConfiguration>(
				"one-of-zone-deployment", i => new OneOfZoneDeploymentConfiguration(i));

			Block.AddParser<ObjectiveSuccessLevel>(
				"objective-success-level", Parse.EnumParser<ObjectiveSuccessLevel>(typeof(ObjectiveSuccessLevel)));
			Block.AddParser<VictoryCondition>("victory-condition", i => new VictoryCondition(i));
			Block.AddParser<ObjectiveSuccessTrigger>("objective-success-trigger", i => new ObjectiveSuccessTrigger(i));

			Block.AddParser<Objective>("units-destroyed-objective", i => new UnitsDestroyedObjective(i));
			Block.AddParser<Objective>("units-in-zone-objective", i => new UnitsInZoneObjective(i));
			Block.AddParser<Objective>("furthest-advance-objective", i => new FurthestAdvanceObjective(i));
			Block.AddParser<Objective>("line-of-fire-objective", i => new LineOfFireObjective(i));
			Block.AddParser<Objective>("units-matched-objective", i => new UnitsMatchedObjective(i));

			Block.AddParser<ArmyConfiguration>("army-configuration", i => new ArmyConfiguration(i));
			Block.AddParser<BoardCompositeMapConfiguration>("map-configuration", i => new BoardCompositeMapConfiguration(i));
			Block.AddParser<Scenario>("scenario", i => new Scenario(i));

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			Factions = (Dictionary<string, Faction>)attributes[(int)Attribute.FACTIONS];
			UnitConfigurations = (Dictionary<string, UnitConfiguration>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
			Wreckage = UnitConfigurations["wreckage"];
			UnitConfigurationLinks = (List<UnitConfigurationLink>)attributes[(int)Attribute.UNIT_CONFIGURATION_LINKS];
			Scenarios = (List<Scenario>)attributes[(int)Attribute.SCENARIOS];
		}
	}
}
