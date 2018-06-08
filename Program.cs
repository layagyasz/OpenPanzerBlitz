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
		static readonly Interface Interface = new Interface(VideoMode.DesktopMode, "PanzerBlitz", Styles.Default);

		public static void Main(string[] args)
		{
			if (args.Length == 4 && args[0] == "lang")
			{
				FileUtils.MungeLanguage(Convert.ToUInt32(args[1]), args[2], args[3]);
			}
			if (args.Length == 1 && args[0] == "remap")
			{
				FileUtils.Remap();
			}

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
							var stream = new SerializationInputStream(compressionStream);
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
								var stream = new SerializationOutputStream(compressionStream);
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
				var fileName = string.Format("Logs/CrashDump-{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
				var log = new StringBuilder();
				log.AppendFormat("[ProgramState]\n{0}\n\n", flowController);
				log.AppendFormat("[StackTrace]\n{0}", e);
				File.WriteAllText(fileName, log.ToString());
				Console.WriteLine("CrashDump written to {0}\n\n{1}", fileName, log);
			}
		}
	}
}
