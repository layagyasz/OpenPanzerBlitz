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
		enum Attribute
		{
			UNIT_MOVEMENT_RULES,
			FACTIONS,
			FACTION_RENDER_DETAILS,
			UNIT_CONFIGURATIONS,
			UNIT_RENDER_DETAILS,
			UNIT_CONFIGURATION_LINKS,
			SCENARIOS,
			TILE_COMPONENT_RULES,
			ENVIRONMENTS,
			TILE_RENDERERS
		};

		public static ushort OnlinePort = 1000;
		public static Player Player =
			new Player((int)DateTime.Now.Ticks, "Player " + DateTime.Now.Ticks.ToString(), true);
		public static Dictionary<string, UnitMovementRules> UnitMovementRules;
		public static Dictionary<string, Faction> Factions;
		public static Dictionary<string, FactionRenderDetails> FactionRenderDetails;
		public static Dictionary<string, UnitConfiguration> UnitConfigurations;
		public static Dictionary<string, UnitRenderDetails> UnitRenderDetails;
		public static List<UnitConfigurationLink> UnitConfigurationLinks;
		public static UnitConfiguration Wreckage;
		public static List<Scenario> Scenarios;
		public static Dictionary<string, TileComponentRules> TileComponentRules;
		public static Dictionary<string, Environment> Environments;
		public static Dictionary<string, TileRenderer> TileRenderers;

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
				ParseBlock.FromFile(path + "/UnitMovementRules.blk"),
				ParseBlock.FromFile(path + "/Factions.blk"),
				ParseBlock.FromFile(path + "/FactionRenderDetails.blk"),
				ParseBlock.FromFile(path + "/Terrain.blk"),
				ParseBlock.FromFile(path + "/Environments.blk"),
				ParseBlock.FromFile(path + "/TerrainRenderers.blk"),
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
			Block.AddParser<WeaponClass>("tile-edge", Parse.EnumBlockParser<WeaponClass>(typeof(TileEdge)));
			Block.AddParser<WeaponClass>(
				"tile-path-overlay", Parse.EnumBlockParser<WeaponClass>(typeof(TilePathOverlay)));
			Block.AddParser<WeaponClass>("tile-base", Parse.EnumBlockParser<WeaponClass>(typeof(TileBase)));
			Block.AddParser<WeaponClass>("weapon-class", Parse.EnumBlockParser<WeaponClass>(typeof(WeaponClass)));
			Block.AddParser<UnitClass>("unit-class", Parse.EnumBlockParser<UnitClass>(typeof(UnitClass)));
			Block.AddParser<UnitStatus>("unit-status", Parse.EnumBlockParser<UnitStatus>(typeof(UnitStatus)));
			Block.AddParser<BlockType>("block-type", Parse.EnumBlockParser<BlockType>(typeof(BlockType)));

			Block.AddParser<MovementCost>("movement-cost", i => new MovementCost(i));
			Block.AddParser<UnitMovementRules>("unit-movement-rules", i => new UnitMovementRules(i));
			Block.AddParser<Faction>("faction", i => new Faction(i));
			Block.AddParser<UnitConfiguration>("unit-configuration", i => new UnitConfiguration(i));
			Block.AddParser<UnitConfigurationLink>("unit-configuration-link", i => new UnitConfigurationLink(i));
			Block.AddParser<Direction>("direction", Parse.EnumBlockParser<Direction>(typeof(Direction)));
			Block.AddParser<TileComponentRules>("tile-component-rules", i => new TileComponentRules(i));
			Block.AddParser<TileRuleSet>("tile-rule-set", i => new TileRuleSet(i));
			Block.AddParser<Environment>("environment", i => new Environment(i));

			Block.AddParsers<object>(MatcherSerializer.Instance.GetParsers());

			Block.AddParser<UnitGroup>("unit-group", i => new UnitGroup(i));
			Block.AddParser<UnitCount>("unit-count", i => new UnitCount(i));
			Block.AddParser<ConvoyMovementAutomator>("convoy-movement-automator", i => new ConvoyMovementAutomator(i));
			Block.AddParser<DeploymentConfiguration>(
				"positional-deployment-configuration", i => new PositionalDeploymentConfiguration(i));
			Block.AddParser<DeploymentConfiguration>(
				"convoy-deployment-configuration", i => new ConvoyDeploymentConfiguration(i));

			Block.AddParser<ObjectiveSuccessLevel>(
				"objective-success-level", Parse.EnumBlockParser<ObjectiveSuccessLevel>(typeof(ObjectiveSuccessLevel)));
			Block.AddParser<VictoryCondition>("victory-condition", i => new VictoryCondition(i));
			Block.AddParser<ObjectiveSuccessTrigger>("objective-success-trigger", i => new ObjectiveSuccessTrigger(i));

			Block.AddParsers<Objective>(ObjectiveSerializer.Instance.GetParsers());

			Block.AddParser<ArmyConfiguration>("army-configuration", i => new ArmyConfiguration(i));
			Block.AddParser<BoardConfiguration>("board-configuration", i => new BoardConfiguration(i));
			Block.AddParser<BoardCompositeMapConfiguration>(
				"map-configuration", i => new BoardCompositeMapConfiguration(i));
			Block.AddParser<Scenario>("scenario", i => new Scenario(i));

			Block.AddParser<FactionRenderDetails>(
				"faction-render-details", i => new FactionRenderDetails(i, Path + "/FactionSymbols/"));
			Block.AddParser<UnitRenderDetails>(
				"unit-render-details", i => new UnitRenderDetails(i, Path + "/UnitSprites/"));
			Block.AddParser<TileRenderer>("tile-renderer", i => new TileRenderer(i));

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			UnitMovementRules = (Dictionary<string, UnitMovementRules>)attributes[(int)Attribute.UNIT_MOVEMENT_RULES];
			Factions = (Dictionary<string, Faction>)attributes[(int)Attribute.FACTIONS];
			FactionRenderDetails =
				(Dictionary<string, FactionRenderDetails>)attributes[(int)Attribute.FACTION_RENDER_DETAILS];
			UnitConfigurations = (Dictionary<string, UnitConfiguration>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
			UnitRenderDetails = (Dictionary<string, UnitRenderDetails>)attributes[(int)Attribute.UNIT_RENDER_DETAILS];
			Wreckage = UnitConfigurations["wreckage"];
			Scenarios = (List<Scenario>)attributes[(int)Attribute.SCENARIOS];
			TileComponentRules =
				(Dictionary<string, TileComponentRules>)attributes[(int)Attribute.TILE_COMPONENT_RULES];
			Environments = (Dictionary<string, Environment>)attributes[(int)Attribute.ENVIRONMENTS];
			TileRenderers = (Dictionary<string, TileRenderer>)attributes[(int)Attribute.TILE_RENDERERS];

			// Emit warnings for units without configured render details.
			foreach (UnitConfiguration unit in UnitConfigurations.Values)
			{
				if (!UnitRenderDetails.ContainsKey(unit.UniqueKey))
					Console.WriteLine(
						"[WARNING]: No render details configured for UnitConfiguration {0}", unit.UniqueKey);
				else if (!File.Exists(UnitRenderDetails[unit.UniqueKey].ImagePath))
					Console.WriteLine(
						"[WARNING]: Image {0} missing for UnitConfiguration {1}",
						UnitRenderDetails[unit.UniqueKey].ImagePath,
						unit.UniqueKey);
			}

			// Emit warnings for factions without configured render details.
			foreach (Faction faction in Factions.Values)
			{
				if (!FactionRenderDetails.ContainsKey(faction.UniqueKey))
					Console.WriteLine(
						"[WARNING]: No render details configured for Faction {0}", faction.UniqueKey);
				else if (!File.Exists(FactionRenderDetails[faction.UniqueKey].ImagePath))
					Console.WriteLine(
						"[WARNING]: Image {0} missing for Faction {1}",
						FactionRenderDetails[faction.UniqueKey].ImagePath,
						faction.UniqueKey);
			}

			// Debug scenario configurations.
			foreach (Scenario s in Scenarios)
			{
				using (MemoryStream m = new MemoryStream())
				{
					try
					{
						SerializationOutputStream stream = new SerializationOutputStream(m);
						stream.Write(s);
					}
					catch (Exception e)
					{
						Console.WriteLine("There was an error writing scenario {0}\n\n{1}", s.Name, e.Message);
					}
					m.Seek(0, SeekOrigin.Begin);
					try
					{
						SerializationInputStream stream = new SerializationInputStream(m);
						Scenario copy = new Scenario(stream);
					}
					catch (Exception e)
					{
						Console.WriteLine("There was an error reading scenario {0}\n\n{1}", s.Name, e);
						Console.WriteLine(new HexDumpWriter(16).Write(m.GetBuffer()));
					}
				}
			}
		}
	}
}
