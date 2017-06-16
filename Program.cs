using System;
using System.Collections.Generic;

using Cardamom.Interface;

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

			Interface I = new Interface(VideoMode.DesktopMode, "PanzerBlitz", Styles.Default);
			I.Screen = new Screen();
			List<Army> armies = new List<Army>();
			GameScreen screen = new GameScreen(I.WindowBounds[2], match.Scenario.Map, match.Armies);
			GameScreenController controller = new GameScreenController(match, screen);
			I.Screen.Add(screen);
			I.Start(false, true);
		}
	}
}
