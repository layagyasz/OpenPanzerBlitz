using System;
using System.Linq;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class ConvoyDeploymentMicrocontroller : DeploymentMicrocontroller
	{
		ConvoyDeployment _Deployment;
		ConvoyDeploymentPage _DeploymentPage;
		Pane _LoadUnitPane;

		public override Deployment Deployment
		{
			get
			{
				return _Deployment;
			}
		}

		public ConvoyDeploymentMicrocontroller(HumanMatchPlayerController Controller, ConvoyDeployment Deployment)
			: base(Controller)
		{
			_Deployment = Deployment;
		}

		public override DeploymentPage MakePage(DeploymentPane Pane, UnitConfigurationRenderer Renderer)
		{
			_DeploymentPage = new ConvoyDeploymentPage(_Deployment, Renderer, Pane);
			_DeploymentPage.OnLoadAction += HandleLoad;
			_DeploymentPage.OnUnloadAction += HandleUnload;
			return _DeploymentPage;
		}

		public override void Clear()
		{
			if (_LoadUnitPane != null)
			{
				_Controller.RemovePane(_LoadUnitPane);
				_LoadUnitPane = null;
			}
		}

		public override void Begin()
		{
			base.Begin();
			HighlightDeploymentArea(null, EventArgs.Empty);
		}

		public override bool Finish()
		{
			var order = new ConvoyOrderDeployOrder(_Deployment, _DeploymentPage.GetConvoyOrder());
			if (!_Controller.ExecuteOrderAndAlert(order))
			{
				_Controller.Alert(order.Validate());
				return false;
			}
			return true;
		}

		public override void End()
		{
			base.End();
			Clear();
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			Clear();
			var o = new EntryTileDeployOrder(_Deployment, Tile);
			if (_Controller.ExecuteOrderAndAlert(o)) HighlightDeploymentArea(null, EventArgs.Empty);
		}

		public override void HandleTileRightClick(Tile Tile)
		{
			Clear();
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			Clear();
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
			Clear();
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
		}

		void HandleLoad(object Sender, EventArgs E)
		{
			if (_DeploymentPage.SelectedUnit != null)
			{
				var units =
					Deployment.Units.Where(i => _DeploymentPage.SelectedUnit.CanLoad(i) == OrderInvalidReason.NONE);
				if (units.Count() > 0)
				{
					Clear();
					var pane = new SelectPane<Unit>("Load Unit", units);
					pane.OnItemSelected += LoadUnit;
					_LoadUnitPane = pane;
					_Controller.AddPane(_LoadUnitPane);
				}
			}
		}

		void HandleUnload(object Sender, EventArgs E)
		{
			UnloadUnit();
		}

		void HighlightDeploymentArea(object Sender, EventArgs E)
		{
			_Controller.Highlight(
				_Controller.Match.GetMap().TilesEnumerable.Where(
					i => _Deployment.Validate(i) == OrderInvalidReason.NONE)
				.Select(i => new Tuple<Tile, Color>(
					i, _Deployment.EntryTile == i
					? HumanMatchPlayerController.HIGHLIGHT_COLORS.Last()
					: HumanMatchPlayerController.HIGHLIGHT_COLORS.First())));
		}

		void LoadUnit(object Sender, ValuedEventArgs<Unit> E)
		{
			LoadUnit(E.Value);
		}

		void LoadUnit(Unit Unit)
		{
			if (_DeploymentPage.SelectedUnit != null)
			{
				var order = new LoadOrder(_DeploymentPage.SelectedUnit, Unit, false);
				if (!_Controller.ExecuteOrderAndAlert(order))
					_Controller.Alert(order.Validate());
			}
			Clear();
		}

		new void UnloadUnit()
		{
			if (_DeploymentPage.SelectedUnit != null)
			{
				var order = new UnloadOrder(_DeploymentPage.SelectedUnit, false);
				if (!_Controller.ExecuteOrderAndAlert(order)) _Controller.Alert(order.Validate().ToString());
			}
		}
	}
}
