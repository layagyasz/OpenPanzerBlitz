using System;
using System.Collections.Generic;
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

		void Clear()
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
			ConvoyOrderDeployOrder order = new ConvoyOrderDeployOrder(_Deployment, _DeploymentPage.GetConvoyOrder());
			if (!_Controller.Match.ExecuteOrder(order))
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
			EntryTileDeployOrder o = new EntryTileDeployOrder(_Deployment, Tile);
			if (_Controller.Match.ExecuteOrder(o)) HighlightDeploymentArea(null, EventArgs.Empty);
			else _Controller.Alert(o.Validate());
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
				IEnumerable<Unit> units =
					Deployment.Units.Where(i => _DeploymentPage.SelectedUnit.CanLoad(i) == OrderInvalidReason.NONE);
				if (units.Count() > 0)
				{
					Clear();
					SelectPane<Unit> pane = new SelectPane<Unit>("Load Unit", units); ;
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
					? HIGHLIGHT_COLORS[HIGHLIGHT_COLORS.Length - 1]
					: HIGHLIGHT_COLORS[0])));
		}

		void LoadUnit(object Sender, ValuedEventArgs<Unit> E)
		{
			LoadUnit(E.Value);
		}

		void LoadUnit(Unit Unit)
		{
			if (_DeploymentPage.SelectedUnit != null)
			{
				LoadOrder order = new LoadOrder(_DeploymentPage.SelectedUnit, Unit, false);
				if (!_Controller.Match.ExecuteOrder(order))
					_Controller.Alert(order.Validate());
			}
			Clear();
		}

		void UnloadUnit()
		{
			if (_DeploymentPage.SelectedUnit != null)
			{
				UnloadOrder order = new UnloadOrder(_DeploymentPage.SelectedUnit, false);
				if (!_Controller.Match.ExecuteOrder(order)) _Controller.Alert(order.Validate().ToString());
			}
		}
	}
}
