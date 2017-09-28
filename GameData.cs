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
		enum Attribute { FACTIONS, UNIT_CONFIGURATIONS, UNIT_RENDER_DETAILS, UNIT_CONFIGURATION_LINKS, SCENARIOS };

		public static ushort OnlinePort = 1000;
		public static Player Player = new Player((int)DateTime.Now.Ticks, "Player " + DateTime.Now.Ticks.ToString(), true);
		public static Dictionary<string, Faction> Factions;
		public static Dictionary<string, UnitConfiguration> UnitConfigurations;
		public static Dictionary<UnitConfiguration, UnitRenderDetails> UnitRenderDetails;
		public static List<UnitConfigurationLink> UnitConfigurationLinks;
		public static UnitConfiguration Wreckage;
		public static List<Scenario> Scenarios;

		public static void Load(string Module)
		{
			string path = "./Modules/" + Module;

			ClassLibrary.Instance.ReadBlock(new ParseBlock(new ParseBlock[]
				{
					ParseBlock.FromFile(path + "/Theme/Fonts.blk"),
						new ParseBlock(
							"class<>",
							"classes",
							Enumerable.Repeat(path + "/Theme/Base.blk", 1)
								.Concat(Directory.EnumerateFiles(
									path + "/Theme/Components", "*", SearchOption.AllDirectories))
								.SelectMany(i => ParseBlock.FromFile(i).Break()))
				}));

			ParseBlock block = new ParseBlock(new ParseBlock[] {
				ParseBlock.FromFile(path + "/Factions.blk"),
				new ParseBlock(
					"unit-configuration<>",
					"unit-configurations",
					Directory.EnumerateFiles(path + "/UnitConfigurations", "*", SearchOption.AllDirectories)
						.SelectMany(i => ParseBlock.FromFile(i).Break())),
				new ParseBlock(
					"unit-render-details<>",
					"unit-render-details",
					Directory.EnumerateFiles(path + "/UnitRenderDetails", "*", SearchOption.AllDirectories)
						.SelectMany(i => ParseBlock.FromFile(i).Break())),
				new ParseBlock(
					"scenario[]",
					"scenarios",
					Directory.EnumerateFiles(path + "/Scenarios", "*", SearchOption.AllDirectories)
						.Select(i => ParseBlock.FromFile(i)))
			});
			Load(path, block);
		}

		static void Load(string Path, ParseBlock Block)
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
			Block.AddParser<UnitRenderDetails>(
				"unit-render-details", i => new UnitRenderDetails(i, Path + "/UnitSprites/"));
			Block.AddParser<UnitConfigurationLink>("unit-configuration-link", i => new UnitConfigurationLink(i));
			Block.AddParser<Direction>("direction", Parse.EnumParser<Direction>(typeof(Direction)));

			Block.AddParser<Matcher<Tile>>("tile-has-coordinate", i => new TileHasCoordinate(i));
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
			Block.AddParser<Matcher<Unit>>("unit-has-position", i => new UnitHasPosition(i));
			Block.AddParser<Matcher<Unit>>("unit-has-configuration", i => new UnitHasConfiguration(i));
			Block.AddParser<Matcher<Unit>>("unit-is-hostile", i => new UnitIsHostile(i));
			Block.AddParser<Matcher<Unit>>(
				"unit-matches-all", i => new CompositeMatcher<Unit>(i, CompositeMatcher<Unit>.AND));
			Block.AddParser<Matcher<Unit>>(
				"unit-matches-any", i => new CompositeMatcher<Unit>(i, CompositeMatcher<Unit>.OR));

			Block.AddParser<UnitGroup>("unit-group", i => new UnitGroup(i));
			Block.AddParser<UnitCount>("unit-count", i => new UnitCount(i));
			Block.AddParser<ConvoyMovementAutomator>("convoy-movement-automator", i => new ConvoyMovementAutomator(i));
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
			Block.AddParser<Objective>("furthest-advance-objective", i => new FurthestAdvanceObjective(i));
			Block.AddParser<Objective>("line-of-fire-objective", i => new LineOfFireObjective(i));
			Block.AddParser<Objective>("units-matched-objective", i => new UnitsMatchedObjective(i));
			Block.AddParser<Objective>("prevent-enemy-objective", i => new PreventEnemyObjective(i));
			Block.AddParser<Objective>("ratio-objective", i => new RatioObjective(i));

			Block.AddParser<ArmyConfiguration>("army-configuration", i => new ArmyConfiguration(i));
			Block.AddParser<BoardConfiguration>("board-configuration", i => new BoardConfiguration(i));
			Block.AddParser<BoardCompositeMapConfiguration>(
				"map-configuration", i => new BoardCompositeMapConfiguration(i));
			Block.AddParser<Scenario>("scenario", i => new Scenario(i));

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			Factions = (Dictionary<string, Faction>)attributes[(int)Attribute.FACTIONS];
			UnitConfigurations = (Dictionary<string, UnitConfiguration>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
			UnitRenderDetails = ((Dictionary<string, UnitRenderDetails>)attributes[(int)Attribute.UNIT_RENDER_DETAILS])
				.Select(i => new KeyValuePair<UnitConfiguration, UnitRenderDetails>(UnitConfigurations[i.Key], i.Value))
				.ToDictionary(i => i.Key, i => i.Value);
			Wreckage = UnitConfigurations["wreckage"];
			Scenarios = (List<Scenario>)attributes[(int)Attribute.SCENARIOS];

			// Emit warnings for units without configured render details.
			foreach (UnitConfiguration unit in UnitConfigurations.Values)
			{
				if (!UnitRenderDetails.ContainsKey(unit))
					Console.WriteLine("[WARNING]: No render details configured for UnitConfiguration {0}", unit.Name);
				else if (!File.Exists(UnitRenderDetails[unit].ImagePath))
					Console.WriteLine(
						"[WARNING]: Image {0} missing for UnitConfiguration {1}",
						UnitRenderDetails[unit].ImagePath,
						unit.Name);
			}
		}
	}
}
