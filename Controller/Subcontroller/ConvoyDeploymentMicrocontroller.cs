using System;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class ConvoyDeploymentMicrocontroller : DeploymentMicrocontroller
	{
		ConvoyDeployment _Deployment;
		ConvoyDeploymentPage _DeploymentPage;

		public override Deployment Deployment
		{
			get
			{
				return _Deployment;
			}
		}
		public override DeploymentPage DeploymentPage
		{
			get
			{
				return _DeploymentPage;
			}
		}

		public ConvoyDeploymentMicrocontroller(
			Match Match,
			GameScreen GameScreen,
			ConvoyDeployment Deployment,
			UnitConfigurationRenderer Renderer)
					: base(Match, GameScreen)
		{
			_Deployment = Deployment;
			_DeploymentPage = new ConvoyDeploymentPage(Deployment, Renderer);
		}

		public override void Begin(Army Army)
		{
			base.Begin(Army);
			HighlightDeploymentArea(null, EventArgs.Empty);
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			EntryTileDeployOrder o = new EntryTileDeployOrder(_Deployment, Tile);
			if (_Match.ExecuteOrder(o)) HighlightDeploymentArea(null, EventArgs.Empty);
			else _GameScreen.Alert(o.Validate().ToString());
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
		}

		void HighlightDeploymentArea(object Sender, EventArgs E)
		{
			Highlight(
				_Match.Map.TilesEnumerable.Where(
					i => _Deployment.Validate(i) == NoDeployReason.NONE)
				.Select(i => new Tuple<Tile, Color>(
					i, _Deployment.EntryTile == i
					? HIGHLIGHT_COLORS[HIGHLIGHT_COLORS.Length - 1]
					: HIGHLIGHT_COLORS[0])));
		}
	}
}
