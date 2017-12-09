using System;
using System.IO;
using System.IO.Compression;

using Cardamom.Interface;
using Cardamom.Serialization;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LocalMatchRecordSelectStateController : PagedProgramStateController
	{
		public LocalMatchRecordSelectStateController()
			: base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = ProgramStateContext;

			MatchRecordSelectScreen scenarioSelect =
				new MatchRecordSelectScreen(
					ProgramContext.ScreenResolution, string.Format("./MatchRecords/{0}", GameData.LoadedModule));
			scenarioSelect.OnMatchRecordSelected += HandleStartScenario;
			scenarioSelect.OnMainMenuButtonClicked += HandleBack;

			return scenarioSelect;
		}

		void HandleStartScenario(object Sender, ValuedEventArgs<FileInfo> E)
		{
			MatchRecord record = null;
			using (FileStream stream = new FileStream(E.Value.FullName, FileMode.Open))
			{
				using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					record = new MatchRecord(new SerializationInputStream(compressionStream));
				}
			}
			MatchContext m = new MatchContext(record.Match);
			MatchAdapter a = m.MakeMatchAdapter();
			MatchRecordReplayPlayerController controller = new MatchRecordReplayPlayerController(a, record);
			foreach (Army army in record.Match.Armies) m.OverridePlayerController(army, controller);

			OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.MATCH, m));
		}
	}
}
