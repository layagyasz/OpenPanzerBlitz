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
		enum Attribute { FACTIONS, UNIT_CONFIGURATIONS, SCENARIOS };

		public static Player Player = new Player("Player " + DateTime.Now.Ticks.ToString());
		public static Dictionary<string, Faction> Factions;
		public static Dictionary<string, UnitConfiguration> UnitConfigurations;
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

			Block.AddParser<WeaponClass>("weapon-class", Parse.EnumParser<WeaponClass>(typeof(WeaponClass)));
			Block.AddParser<UnitClass>("unit-class", Parse.EnumParser<UnitClass>(typeof(UnitClass)));
			Block.AddParser<Faction>("faction", i => new Faction(i));
			Block.AddParser<UnitConfiguration>("unit-configuration", i => new UnitConfiguration(i));
			Block.AddParser<Direction>("direction", Parse.EnumParser<Direction>(typeof(Direction)));

			Block.AddParser<TileElevation>("tile-elevation", i => new TileElevation(i));
			Block.AddParser<TileWithin>("tile-within", i => new TileWithin(i));
			Block.AddParser<TileOnEdge>("tile-on-edge", i => new TileOnEdge(i));
			Block.AddParser<DistanceFromUnit>("distance-from-unit", i => new DistanceFromUnit(i));
			Block.AddParser<Polygon>("zone", i => new Polygon(i));
			Block.AddParser<Coordinate>("coordinate", i => new Coordinate(i));
			Block.AddParser<CompositeMatcher>("matches-all", i => new CompositeMatcher(i, CompositeMatcher.AND));
			Block.AddParser<CompositeMatcher>("matches-any", i => new CompositeMatcher(i, CompositeMatcher.OR));

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

			Block.AddParser<ArmyConfiguration>("army-configuration", i => new ArmyConfiguration(i));
			Block.AddParser<MapConfiguration>("map-configuration", i => new MapConfiguration(i));
			Block.AddParser<Scenario>("scenario", i => new Scenario(i));

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			Factions = (Dictionary<string, Faction>)attributes[(int)Attribute.FACTIONS];
			UnitConfigurations = (Dictionary<string, UnitConfiguration>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
			Wreckage = UnitConfigurations["wreckage"];
			Scenarios = (List<Scenario>)attributes[(int)Attribute.SCENARIOS];
		}
	}
}
