using System;

using Cardamom.Interface;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class MatchLobbyStateController : PagedProgramStateController
	{
		MatchLobbyController _Controller;
		ChatController _ChatController;

		readonly EventBuffer<ValuedEventArgs<Scenario>> _LaunchBuffer = new EventBuffer<ValuedEventArgs<Scenario>>();

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
			context.Lobby.OnLaunched += _LaunchBuffer.Hook<ValuedEventArgs<Scenario>>(HandleLaunch).Invoke;
			screen.OnPulse += HandlePulse;
			screen.OnMainMenuButtonClicked += HandleBack;
			return screen;
		}

		void HandleLaunch(object Sender, ValuedEventArgs<Scenario> E)
		{
			OnProgramStateTransition(
				this, new ProgramStateTransitionEventArgs(
					ProgramState.MATCH, ((MatchLobbyContext)_Context).MakeMatchContext(E.Value)));
		}

		void HandlePulse(object Sender, EventArgs E)
		{
			_LaunchBuffer.DispatchEvents();
		}
	}
}
