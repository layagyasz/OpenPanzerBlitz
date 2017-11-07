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
				if (_Controller.ExecuteOrderAndAlert(new PositionalDeployOrder(unit, Tile)))
				{
					_Controller.SelectUnit(unit);
					_DeploymentPage.Remove(unit);
					HighlightDeploymentArea(null, EventArgs.Empty);
				}
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (_Controller.CurrentTurn.Army == Unit.Army) _Controller.SelectUnit(Unit);
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
			if (_Controller.ExecuteOrderAndAlert(new PositionalDeployOrder(Unit, null)))
			{
				_DeploymentPage.Add(Unit);
				HighlightDeploymentArea(null, EventArgs.Empty);
			}
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
			// Load/Unload
			if (Key == Keyboard.Key.L) LoadUnit();
			else if (Key == Keyboard.Key.U) UnloadUnit();
			// Mount/Dismount
			else if (Key == Keyboard.Key.M) Mount();
			else if (Key == Keyboard.Key.D) Dismount();
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
