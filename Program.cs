using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
			ProgramFlowController flowController = null;
			try
			{
				ClassLibrary.Instance.ReadBlock(new ParseBlock(new ParseBlock[]
				{
					ParseBlock.FromFile("./Theme/Fonts.blk"),
						new ParseBlock(
							"class<>",
							"classes",
							Enumerable.Repeat("./Theme/Base.blk", 1)
								.Concat(Directory.EnumerateFiles(
									"./Theme/Components", "*", SearchOption.AllDirectories))
								.SelectMany(i => ParseBlock.FromFile(i).Break()))
				}));
				GameData.Load("./BLKConfigurations");

				Interface.Screen = new Screen();

				flowController = new ProgramFlowController(Interface);
				flowController.EnterState(ProgramState.LANDING, null);

				Interface.Start(false, true);
			}
			catch (Exception e)
			{
				string fileName = string.Format("CrashDump-{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
				StringBuilder log = new StringBuilder();
				log.AppendFormat("[ProgramState]\n{0}\n\n", flowController);
				log.AppendFormat("[StackTrace]\n{0}", e);
				File.WriteAllText("Logs/" + fileName, log.ToString());
			}
		}
	}
}
