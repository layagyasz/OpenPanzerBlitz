using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class MatchStateController : ProgramStateController
	{
		MatchContext _Context;
		MatchController _MatchController;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = (MatchContext)ProgramStateContext;

			UnitConfigurationRenderer renderer = new UnitConfigurationRenderer(
				_Context.Match.Scenario, 1024, 128, new Font("Compacta Std Regular.otf"));
			HashSet<Army> armies = new HashSet<Army>(_Context.GetArmies());

			MatchScreen screen = new MatchScreen(
				ProgramContext.ScreenResolution,
				_Context.Match.Map,
				TileRenderer.SUMMER_STEPPE,
				_Context.Match.Armies.Select(i => new ArmyView(i, renderer)));
			HumanMatchPlayerController controller =
				new HumanMatchPlayerController(
					_Context.MakeMatchAdapter(), armies, renderer, screen, ProgramContext.KeyController);

			Dictionary<Army, MatchPlayerController> playerControllers = new Dictionary<Army, MatchPlayerController>();
			foreach (Army a in _Context.Match.Armies)
			{
				playerControllers.Add(a, controller);
			}
			_MatchController = new MatchController(_Context.Match, playerControllers, _Context.IsHost);
			screen.OnPulse += (sender, e) => _Context.Match.DoBufferedOrders();
			_Context.Match.Start();

			return screen;
		}

		public override string ToString()
		{
			return string.Format("[MatchStateController: Context={0}]", _Context);
		}
	}
}
