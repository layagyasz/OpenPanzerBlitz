using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Cardamom.Interface;
using Cardamom.Serialization;

using SFML.Window;

namespace PanzerBlitz
{
	class MainClass
	{
		static Interface Interface = new Interface(VideoMode.DesktopMode, "PanzerBlitz", Styles.Default);

		public static void Main(string[] args)
		{
			string module = "Default";
			string modulePath = "./Modules/" + module;
			string moduleFile = modulePath + ".mod";

			bool mungeModule = false;
			bool tryLoadMungedModule = false;

			ProgramFlowController flowController = null;
			try
			{
				if (File.Exists(moduleFile) && tryLoadMungedModule)
				{
					using (FileStream fileStream = new FileStream(moduleFile, FileMode.Open))
					{
						using (GZipStream compressionStream = new GZipStream(fileStream, CompressionMode.Decompress))
						{
							SerializationInputStream stream = new SerializationInputStream(compressionStream);
							GameData.Load(module, stream);
						}
					}
				}
				else
				{
					GameData.Load(module);

					if (mungeModule)
					{
						using (FileStream fileStream = new FileStream(moduleFile, FileMode.Create))
						{
							using (GZipStream compressionStream = new GZipStream(fileStream, CompressionLevel.Optimal))
							{
								SerializationOutputStream stream = new SerializationOutputStream(compressionStream);
								GameData.Serialize(stream);
							}
						}
					}
				}

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
