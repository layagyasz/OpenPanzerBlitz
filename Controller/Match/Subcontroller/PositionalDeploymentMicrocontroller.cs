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
			_DeploymentPage = new PositionalDeploymentPage(_Deployment, Renderer);
			_DeploymentPage.OnSelectedStack += HighlightDeploymentArea;
			return _DeploymentPage;
		}

		public override void Begin()
		{
			base.Begin();
			HighlightDeploymentArea(null, EventArgs.Empty);
		}

		public override bool CanLoad()
		{
			return _Controller.SelectedUnit.Position.Units.Any(
				i => _Controller.SelectedUnit.CanLoad(i, false) == OrderInvalidReason.NONE);
		}

		public override bool CanUnload()
		{
			return _Controller.SelectedUnit.CanUnload(false) == OrderInvalidReason.NONE;
		}

		public override bool CanDismount()
		{
			return _Controller.SelectedUnit.CanDismount() == OrderInvalidReason.NONE;
		}

		public override bool CanMount()
		{
			return _Controller.SelectedUnit.CanMount(true) == OrderInvalidReason.NONE;
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_DeploymentPage.SelectedStack != null)
			{
				var unit = _DeploymentPage.Peek();
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
			switch (Key)
			{
				case Keyboard.Key.L: _Controller.LoadUnit(false); break;
				case Keyboard.Key.U: _Controller.UnloadUnit(); break;
				case Keyboard.Key.M: _Controller.Mount(); break;
				case Keyboard.Key.D: _Controller.Dismount(); break;
			}
		}

		void HighlightDeploymentArea(object Sender, EventArgs E)
		{
			if (_DeploymentPage.SelectedStack != null)
			{
				_Controller.Highlight(_DeploymentPage.SelectedStack.Peek().GetFieldOfDeployment(
					_Controller.Match.GetMap().TilesEnumerable).Select(
						i => new Tuple<Tile, Color>(i, HumanMatchPlayerController.HIGHLIGHT_COLORS.First())));
			}
			else _Controller.UnHighlight();
		}
	}
}
