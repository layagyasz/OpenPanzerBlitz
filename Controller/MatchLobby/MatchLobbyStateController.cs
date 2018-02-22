using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class MatchLobbyStateController : PagedProgramStateController
	{
		MatchLobbyController _Controller;
		ChatController _ChatController;

		bool _Launch;

		public MatchLobbyStateController()
			: base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = ProgramStateContext;
			var context = (MatchLobbyContext)_Context;

			var screen = new MatchLobbyScreen(
				ProgramContext.ScreenResolution,
				context.IsHost,
				context.Lobby,
				context.Chat,
				GameData.Player,
				GameData.Scenarios);
			_Controller = new MatchLobbyController(context.MakeMatchLobbyAdapter(), screen);
			_ChatController = new ChatController(context.MakeChatAdapter(), screen.ChatView, GameData.Player);
			context.Lobby.OnLaunched += HandleLaunch;
			screen.OnPulse += HandlePulse;
			screen.OnMainMenuButtonClicked += HandleBack;
			return screen;
		}

		void HandleLaunch(object Sender, EventArgs E)
		{
			_Launch = true;
		}

		void HandlePulse(object Sender, EventArgs E)
		{
			if (_Launch) OnProgramStateTransition(
				this, new ProgramStateTransitionEventArgs(
					ProgramState.MATCH, ((MatchLobbyContext)_Context).MakeMatchContext()));
		}
	}
}
