using System;
using System.Collections.Generic;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class MatchStateController : ProgramStateController
	{
		MatchContext _Context;
		MatchController _MatchController;

		EventBuffer<EventArgs> _MatchEndBuffer;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = (MatchContext)ProgramStateContext;
			_MatchEndBuffer = new EventBuffer<EventArgs>();

			var renderer = new UnitConfigurationRenderer(
				_Context.Match.Scenario,
				GameData.UnitRenderDetails,
				128,
				1024,
				ClassLibrary.Instance.GetFont("compacta"));
			var factionRenderer =
				new FactionRenderer(_Context.Match.Scenario, GameData.FactionRenderDetails, 512, 1024);
			var armies = new HashSet<Army>(_Context.GetPlayerControlledArmies());

			var screen = new MatchScreen(
				ProgramContext.ScreenResolution,
				_Context.Match,
				armies,
				GameData.TileRenderers[_Context.Match.Scenario.Environment.UniqueKey],
				renderer,
				factionRenderer);
			var controller =
				new HumanMatchPlayerController(
					_Context.MakeMatchAdapter(), armies, renderer, screen, ProgramContext.KeyController);

			var playerControllers = new Dictionary<Army, MatchPlayerController>();
			foreach (Army a in _Context.Match.Armies)
			{
				// var controller = new AIMatchPlayerController(_Context.MakeMatchAdapter(), a);
				var overrideController = _Context.GetOverridePlayerController(a);
				playerControllers.Add(a, overrideController ?? controller);
			}
			_MatchController = new MatchController(_Context.Match, playerControllers);
			screen.OnPulse += HandlePulse;
			_Context.Match.OnMatchEnded += _MatchEndBuffer.Hook<EventArgs>(HandleMatchEnd).Invoke;
			_Context.Match.Start();

			return screen;
		}

		void HandlePulse(object Sender, EventArgs E)
		{
			_Context.Match.DoBufferedOrders();
			_MatchEndBuffer.DispatchEvents();
		}

		void HandleMatchEnd(object Sender, EventArgs E)
		{
			_MatchController.Unhook();
			OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.MATCH_END, _Context));
		}

		public override string ToString()
		{
			return string.Format("[MatchStateController: Context={0}]", _Context);
		}
	}
}
