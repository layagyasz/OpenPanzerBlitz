using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Planar;
using Cardamom.Serialization;
using Cardamom.Utilities.Markov;

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
			TILE_RENDERERS,
			NAME_GENERATORS,
			MATCH_SETTINGS
		};

		public static ushort OnlinePort = 1000;
		public static Player Player =
			new Player((int)DateTime.Now.Ticks, "Player " + DateTime.Now.Ticks.ToString(), true);
		public static string LoadedModule;

		public static Dictionary<string, UnitMovementRules> UnitMovementRules;
		public static Dictionary<string, TileComponentRules> TileComponentRules;
		public static Dictionary<string, Environment> Environments;
		public static Dictionary<string, Faction> Factions;
		public static Dictionary<string, FactionRenderDetails> FactionRenderDetails;
		public static Dictionary<string, UnitConfiguration> UnitConfigurations;
		public static Dictionary<string, UnitRenderDetails> UnitRenderDetails;
		public static List<UnitConfigurationLink> UnitConfigurationLinks;
		public static UnitConfiguration Wreckage;
		public static List<Scenario> Scenarios;
		public static Dictionary<string, TileRenderer> TileRenderers;
		public static Dictionary<string, MarkovGenerator<char>> NameGenerators;
		public static Dictionary<string, MatchSetting> MatchSettings;

		public static void Load(string Module)
		{
			LoadedModule = Module;
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

			var block = new ParseBlock(new ParseBlock[] {
				ParseBlock.FromFile(path + "/UnitMovementRules.blk"),
				ParseBlock.FromFile(path + "/Factions.blk"),
				ParseBlock.FromFile(path + "/FactionRenderDetails.blk"),
				ParseBlock.FromFile(path + "/Terrain.blk"),
				ParseBlock.FromFile(path + "/Environments.blk"),
				ParseBlock.FromFile(path + "/TerrainRenderers.blk"),
				ParseBlock.FromFile(path + "/NameGenerators.blk"),
				ParseBlock.FromFile(path + "/MatchSettings.blk"),
				new ParseBlock(
					"unit-configuration<>",
					"unit-configurations",
					Directory.EnumerateFiles(path + "/UnitConfigurations", "*", SearchOption.AllDirectories)
						.SelectMany(i => ParseBlock.FromFile(i).Break())),
				new ParseBlock(
					"unit-configuration-link[]",
					"unit-configuration-links",
					Directory.EnumerateFiles(path + "/UnitConfigurationLinks", "*", SearchOption.AllDirectories)
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
			Block.AddParser<Coordinate>(typeof(Coordinate));
			Block.AddParser<WeaponClass>(typeof(TileEdge));
			Block.AddParser<WeaponClass>(typeof(TilePathOverlay));
			Block.AddParser<WeaponClass>(typeof(TileBase));
			Block.AddParser<WeaponClass>(typeof(WeaponClass));
			Block.AddParser<UnitClass>(typeof(UnitClass));
			Block.AddParser<UnitStatus>(typeof(UnitStatus));
			Block.AddParser<BlockType>(typeof(BlockType));
			Block.AddParser<Front>(typeof(Front));
			Block.AddParser<Weapon>(typeof(Weapon));

			Block.AddParser<MovementRule>(typeof(MovementRule));
			Block.AddParser<UnitMovementRules>(typeof(UnitMovementRules));
			Block.AddParser<Faction>(typeof(Faction));
			Block.AddParser<UnitConfiguration>(typeof(UnitConfiguration));
			Block.AddParser<UnitConfigurationLink>(typeof(UnitConfigurationLink));
			Block.AddParser<Direction>(typeof(Direction));
			Block.AddParser<TileComponentRules>(typeof(TileComponentRules));
			Block.AddParser<TileRuleSet>(typeof(TileRuleSet));
			Block.AddParser<Environment>(typeof(Environment));

			Block.AddParsers<object>(MatcherSerializer.Instance.GetParsers());

			Block.AddParser<UnitGroup>(typeof(UnitGroup));
			Block.AddParser<UnitCount>(typeof(UnitCount));
			Block.AddParser<ConvoyMovementAutomator>(typeof(ConvoyMovementAutomator));
			Block.AddParser<DeploymentConfiguration>(typeof(PositionalDeploymentConfiguration));
			Block.AddParser<DeploymentConfiguration>(typeof(ConvoyDeploymentConfiguration));

			Block.AddParser<ObjectiveSuccessLevel>(typeof(ObjectiveSuccessLevel));
			Block.AddParser<VictoryCondition>(typeof(VictoryCondition));
			Block.AddParser<ObjectiveSuccessTrigger>(typeof(ObjectiveSuccessTrigger));

			Block.AddParsers<Objective>(ObjectiveSerializer.Instance.GetParsers());

			Block.AddParser<ArmyConfiguration>(typeof(ArmyConfiguration));
			Block.AddParser<BoardConfiguration>(typeof(BoardConfiguration));
			Block.AddParser<BoardCompositeMapConfiguration>(
				"map-configuration", i => new BoardCompositeMapConfiguration(i));
			Block.AddParser<Scenario>(typeof(Scenario));

			Block.AddParser<FactionRenderDetails>(
				"faction-render-details", i => new FactionRenderDetails(i, Path + "/FactionSymbols/"));
			Block.AddParser<UnitRenderDetails>(
				"unit-render-details", i => new UnitRenderDetails(i, Path + "/UnitSprites/"));
			Block.AddParser<TileRenderer>(typeof(TileRenderer));
			Block.AddParser<MapGeneratorConfiguration>(typeof(MapGeneratorConfiguration));
			Block.AddParser<MatchSetting>(typeof(MatchSetting));
			Block.AddParser<MarkovGenerator<char>>(
				"name-generator", i => FileUtils.LoadLanguage(Path + "/NameGenerators/" + i.String));

			var attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			UnitMovementRules = (Dictionary<string, UnitMovementRules>)attributes[(int)Attribute.UNIT_MOVEMENT_RULES];
			TileComponentRules =
				(Dictionary<string, TileComponentRules>)attributes[(int)Attribute.TILE_COMPONENT_RULES];
			Environments = (Dictionary<string, Environment>)attributes[(int)Attribute.ENVIRONMENTS];
			Factions = (Dictionary<string, Faction>)attributes[(int)Attribute.FACTIONS];
			FactionRenderDetails =
				(Dictionary<string, FactionRenderDetails>)attributes[(int)Attribute.FACTION_RENDER_DETAILS];
			UnitConfigurations = (Dictionary<string, UnitConfiguration>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
			UnitConfigurationLinks = (List<UnitConfigurationLink>)attributes[(int)Attribute.UNIT_CONFIGURATION_LINKS];
			UnitRenderDetails = (Dictionary<string, UnitRenderDetails>)attributes[(int)Attribute.UNIT_RENDER_DETAILS];
			Wreckage = UnitConfigurations["wreckage"];
			Scenarios = (List<Scenario>)attributes[(int)Attribute.SCENARIOS];
			TileRenderers = (Dictionary<string, TileRenderer>)attributes[(int)Attribute.TILE_RENDERERS];
			NameGenerators = (Dictionary<string, MarkovGenerator<char>>)attributes[(int)Attribute.NAME_GENERATORS];
			MatchSettings = (Dictionary<string, MatchSetting>)attributes[(int)Attribute.MATCH_SETTINGS];

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
						var stream = new SerializationOutputStream(m);
						stream.Write(s);
					}
					catch (Exception e)
					{
						Console.WriteLine("There was an error writing scenario {0}\n\n{1}", s.Name, e.Message);
					}
					m.Seek(0, SeekOrigin.Begin);
					try
					{
						var stream = new SerializationInputStream(m);
						var copy = new Scenario(stream);
					}
					catch (Exception e)
					{
						Console.WriteLine("There was an error reading scenario {0}\n\n{1}", s.Name, e);
						Console.WriteLine(new HexDumpWriter(16).Write(m.GetBuffer()));
					}
				}
			}
		}

		public static void Load(string Module, SerializationInputStream Stream)
		{
			LoadedModule = Module;
			string Path = "./Modules/" + Module;

			ClassLibrary.Instance.ReadBlock(new ParseBlock(new ParseBlock[]
				{
					ParseBlock.FromFile(Path + "/Theme/Fonts.blk"),
						new ParseBlock(
							"class<>",
							"classes",
							Enumerable.Repeat(Path + "/Theme/Base.blk", 1)
								.Concat(Directory.EnumerateFiles(
									Path + "/Theme/Components", "*", SearchOption.AllDirectories))
								.SelectMany(i => ParseBlock.FromFile(i).Break()))
				}));

			UnitMovementRules = Stream.ReadEnumerable(
				i => i.ReadObject(j => new UnitMovementRules(j), false, true)).ToDictionary(i => i.UniqueKey);
			TileComponentRules = Stream.ReadEnumerable(
				i => i.ReadObject(j => new TileComponentRules(j), false, true)).ToDictionary(i => i.UniqueKey);
			Environments = Stream.ReadEnumerable(
				i => i.ReadObject(j => new Environment(j), false, true)).ToDictionary(i => i.UniqueKey);
			Factions = Stream.ReadEnumerable(
				i => i.ReadObject(j => new Faction(j), false, true)).ToDictionary(i => i.UniqueKey);
			FactionRenderDetails = Stream.ReadEnumerable(
				i => new KeyValuePair<string, FactionRenderDetails>(
					i.ReadString(),
					new FactionRenderDetails(i, Path + "/FactionSymbols/"))).ToDictionary(i => i.Key, i => i.Value);
			UnitConfigurations = Stream.ReadEnumerable(
				i => i.ReadObject(j => new UnitConfiguration(j), false, true)).ToDictionary(i => i.UniqueKey);
			UnitRenderDetails = Stream.ReadEnumerable(
				i => new KeyValuePair<string, UnitRenderDetails>(
					i.ReadString(),
					new UnitRenderDetails(i, Path + "/UnitSprites/"))).ToDictionary(i => i.Key, i => i.Value);
			UnitConfigurationLinks = Stream.ReadEnumerable(i => new UnitConfigurationLink(i)).ToList();
			Scenarios = Stream.ReadEnumerable(i => new Scenario(i)).ToList();
			TileRenderers = Stream.ReadEnumerable(i => new TileRenderer(i)).ToDictionary(i => i.UniqueKey);
			NameGenerators = Stream.ReadEnumerable(
				i => new KeyValuePair<string, MarkovGenerator<char>>(Stream.ReadString(), new MarkovGenerator<char>(i)))
								   .ToDictionary(i => i.Key, i => i.Value);
			MatchSettings = Stream.ReadEnumerable(i => new MatchSetting(i)).ToDictionary(i => i.UniqueKey);
			Wreckage = UnitConfigurations["wreckage"];
		}

		public static void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitMovementRules, i => Stream.Write(i.Value, false, true));
			Stream.Write(TileComponentRules, i => Stream.Write(i.Value, false, true));
			Stream.Write(Environments, i => Stream.Write(i.Value, false, true));
			Stream.Write(Factions, i => Stream.Write(i.Value, false, true));
			Stream.Write(FactionRenderDetails, i =>
			{
				Stream.Write(i.Key);
				Stream.Write(i.Value);
			});
			Stream.Write(UnitConfigurations, i => Stream.Write(i.Value, false, true));
			Stream.Write(UnitRenderDetails, i =>
			{
				Stream.Write(i.Key);
				Stream.Write(i.Value);
			});
			Stream.Write(UnitConfigurationLinks);
			Stream.Write(Scenarios);
			Stream.Write(TileRenderers, i => Stream.Write(i.Value));
			Stream.Write(NameGenerators, i => { Stream.Write(i.Key); Stream.Write(i.Value); });
			Stream.Write(MatchSettings, i => Stream.Write(i.Value));
		}
	}
}
