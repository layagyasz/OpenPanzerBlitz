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
				GameData.Load("Default");

				Interface.Screen = new Screen();

				flowController = new ProgramFlowController(Interface);
				flowController.EnterState(ProgramState.LANDING, null);

				Interface.Start(false, true);
			}
			catch (Exception e)
			{
				string fileName = string.Format("Logs/CrashDump-{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
				StringBuilder log = new StringBuilder();
				log.AppendFormat("[ProgramState]\n{0}\n\n", flowController);
				log.AppendFormat("[StackTrace]\n{0}", e);
				File.WriteAllText(fileName, log.ToString());
				Console.WriteLine("CrashDump written to {0}\n\n{1}", fileName, log);
			}
		}
	}
}
