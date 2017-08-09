using System;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class PositionalDeploymentMicrocontroller : DeploymentMicrocontroller
	{
		PositionalDeployment _Deployment;
		PositionalDeploymentPage _DeploymentPage;

		public override Deployment Deployment
		{
			get
			{
				return _Deployment;
			}
		}

		public PositionalDeploymentMicrocontroller(
			MatchAdapter Match,
			GameScreen GameScreen,
			PositionalDeployment Deployment)
			: base(Match, GameScreen)
		{
			_Deployment = Deployment;
		}

		public override DeploymentPage MakePage(DeploymentPane Pane, UnitConfigurationRenderer Renderer)
		{
			_DeploymentPage = new PositionalDeploymentPage(_Deployment, Renderer, Pane);
			_DeploymentPage.OnSelectedStack += HighlightDeploymentArea;
			return _DeploymentPage;
		}

		public override void Begin(Army Army)
		{
			base.Begin(Army);
			HighlightDeploymentArea(null, EventArgs.Empty);
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_DeploymentPage.SelectedStack != null)
			{
				Unit unit = _DeploymentPage.Peek();
				PositionalDeployOrder o = new PositionalDeployOrder(unit, Tile);
				if (_Match.ExecuteOrder(o))
				{
					_DeploymentPage.Remove(unit);
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
			PositionalDeployOrder o = new PositionalDeployOrder(Unit, null);
			if (_Match.ExecuteOrder(o))
			{
				_DeploymentPage.Add(Unit);
				HighlightDeploymentArea(null, EventArgs.Empty);
			}
		}

		public override void HandleKeyPress(Keyboard.Key key)
		{
		}

		void HighlightDeploymentArea(object Sender, EventArgs E)
		{
			if (_DeploymentPage.SelectedStack != null)
			{
				Highlight(_DeploymentPage.SelectedStack.Peek().GetFieldOfDeployment(
					_Match.GetMap().TilesEnumerable).Select(i => new Tuple<Tile, Color>(i, HIGHLIGHT_COLORS[0])));
			}
			else UnHighlight();
		}
	}
}
