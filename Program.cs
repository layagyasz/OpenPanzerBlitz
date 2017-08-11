using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Serialization;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	class MainClass
	{
		static Interface Interface = new Interface(VideoMode.DesktopMode, "PanzerBlitz", Styles.Default);

		public static void Main(string[] args)
		{
			ClassLibrary.Instance.ReadBlock(new ParseBlock(new ParseBlock[] {
				ParseBlock.FromFile("./Theme/Fonts.blk"),
				new ParseBlock(
					"class<>",
					"classes",
					Enumerable.Repeat("./Theme/Base.blk", 1)
						.Concat(Directory.EnumerateFiles("./Theme/Components", "*", SearchOption.AllDirectories))
						.SelectMany(i => ParseBlock.FromFile(i).Break()))
			}));
			GameData.Load("./BLKConfigurations");

			Interface.Screen = new Screen();

			ProgramFlowController flowController = new ProgramFlowController(Interface);
			flowController.EnterState(ProgramState.LANDING, null);

			Interface.Start(false, true);
		}
	}
}
