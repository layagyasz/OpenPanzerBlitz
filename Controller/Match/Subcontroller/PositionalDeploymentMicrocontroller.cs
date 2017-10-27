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
			HumanMatchPlayerController Controller, PositionalDeployment Deployment)
			: base(Controller)
		{
			_Deployment = Deployment;
		}

		public override DeploymentPage MakePage(DeploymentPane Pane, UnitConfigurationRenderer Renderer)
		{
			_DeploymentPage = new PositionalDeploymentPage(_Deployment, Renderer, Pane);
			_DeploymentPage.OnSelectedStack += HighlightDeploymentArea;
			return _DeploymentPage;
		}

		public override void Begin()
		{
			base.Begin();
			HighlightDeploymentArea(null, EventArgs.Empty);
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_DeploymentPage.SelectedStack != null)
			{
				Unit unit = _DeploymentPage.Peek();
				PositionalDeployOrder o = new PositionalDeployOrder(unit, Tile);
				if (_Controller.Match.ExecuteOrder(o))
				{
					_DeploymentPage.Remove(unit);
					HighlightDeploymentArea(null, EventArgs.Empty);
				}
				else _Controller.Alert(o.Validate());
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
			if (_Controller.Match.ExecuteOrder(o))
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
				_Controller.Highlight(_DeploymentPage.SelectedStack.Peek().GetFieldOfDeployment(
					_Controller.Match.GetMap().TilesEnumerable).Select(
						i => new Tuple<Tile, Color>(i, HIGHLIGHT_COLORS[0])));
			}
			else _Controller.UnHighlight();
		}
	}
}
