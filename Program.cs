using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	class MainClass
	{
		static void Match_OnMatchEnded(object sender, EventArgs e)
		{

		}

		static Interface Interface = new Interface(VideoMode.DesktopMode, "PanzerBlitz", Styles.Default);

		public static void Main(string[] args)
		{
			ClassLibrary.Instance.ReadFile("./Theme.blk");
			Interface.Screen = new Screen();

			bool edit = false;
			if (edit)
			{
				GameScreen screen = new GameScreen(
					Interface.WindowBounds[2],
					new MapView(new Map(11, 33), TileRenderer.SUMMER_STEPPE),
					new ArmyView[] { });
				EditController controller = new EditController(screen);

				Interface.Screen.Add(screen);
			}
			else
			{
				GameData.Load("./BLKConfigurations");

				ScenarioSelectScreen scenarioSelect =
					new ScenarioSelectScreen(Interface.WindowBounds[2], GameData.Scenarios);
				scenarioSelect.OnScenarioSelected += StartScenario;
				Interface.Screen.Add(scenarioSelect);
			}
			Interface.Start(false, true);
		}

		static void StartScenario(object Sender, ValueChangedEventArgs<Scenario> E)
		{
			Match match = new Match(E.Value);
			UnitConfigurationRenderer renderer = new UnitConfigurationRenderer(
				match.Scenario, 1024, 128, new Font("Compacta Std Regular.otf"));
			GameScreen screen = new GameScreen(
				Interface.WindowBounds[2],
				new MapView(match.Map, TileRenderer.SUMMER_STEPPE),
				match.Armies.Select(i => new ArmyView(i, renderer)));
			HumanGamePlayerController controller =
				new HumanGamePlayerController(new LocalMatchAdapter(match), renderer, screen, Interface.KeyController);
			Dictionary<Army, GamePlayerController> playerControllers = new Dictionary<Army, GamePlayerController>();
			foreach (Army a in match.Armies) playerControllers.Add(a, controller);
			GameController gameController = new GameController(match, playerControllers);

			match.OnMatchEnded += MatchEnded;

			Interface.Screen.Clear();
			Interface.Screen.Add(screen);
		}

		static void MatchEnded(object Sender, EventArgs E)
		{
			Match match = (Match)Sender;
			Console.WriteLine(
				string.Join(
					", ", match.GetArmyObjectiveSuccessLevels().Select(i => string.Format("{0}:{1}", i.Key, i.Value))));
		}
	}
}
