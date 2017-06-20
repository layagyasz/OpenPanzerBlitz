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
		public static void Main(string[] args)
		{
			ClassLibrary.Instance.ReadFile("./Theme.blk");
			GameData gameData = new GameData("./BLKConfigurations");

			Match match = new Match(gameData.Scenarios[0]);
			UnitConfigurationRenderer renderer = new UnitConfigurationRenderer(
				gameData.Scenarios[0], 1024, 128, new Font("Compacta Std Regular.otf"));

			Interface I = new Interface(VideoMode.DesktopMode, "PanzerBlitz", Styles.Default);
			I.Screen = new Screen();
			List<Army> armies = new List<Army>();
			GameScreen screen = new GameScreen(
				I.WindowBounds[2], match.Scenario.Map, match.Armies.Select(i => new ArmyView(i, renderer)));
			GameScreenController controller = new GameScreenController(match, renderer, screen);
			I.Screen.Add(screen);
			I.Start(false, true);
		}
	}
}
