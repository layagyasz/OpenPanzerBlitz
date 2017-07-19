using System;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class DeploymentController : BaseController
	{
		DeploymentPane _DeploymentPane;

		UnitConfigurationRenderer _Renderer;

		public DeploymentController(Match Match, UnitConfigurationRenderer Renderer, GameScreen GameScreen)
			: base(Match, GameScreen)
		{
			_Renderer = Renderer;
		}

		public override void Begin(Army Army)
		{
			base.Begin(Army);
			_DeploymentPane = new DeploymentPane(Army, _Renderer);
			_DeploymentPane.OnSelectedStack += HighlightDeploymentArea;
			_GameScreen.AddPane(_DeploymentPane);
		}

		public override void End()
		{
			base.End();
			_GameScreen.RemovePane(_DeploymentPane);
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_DeploymentPane.SelectedStack != null)
			{
				Unit unit = _DeploymentPane.Peek();
				DeployOrder o = new DeployOrder(unit, Tile);
				if (_Match.ExecuteOrder(o))
				{
					_DeploymentPane.Remove(unit);
					HighlightDeploymentArea(null, EventArgs.Empty);
				}
				else _GameScreen.Alert(o.Validate().ToString());
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
			DeployOrder o = new DeployOrder(Unit, null);
			if (_Match.ExecuteOrder(o)) _DeploymentPane.Add(Unit);
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
		}

		void HighlightDeploymentArea(object Sender, EventArgs E)
		{
			if (_DeploymentPane.SelectedStack != null)
			{
				Highlight(_DeploymentPane.SelectedStack.Peek().GetFieldOfDeployment(
					_Match.Map.TilesEnumerable).Select(i => new Tuple<Tile, Color>(i, HIGHLIGHT_COLORS[0])));
			}
			else UnHighlight();
		}
	}
}
