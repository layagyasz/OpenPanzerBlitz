using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class DeploymentController : BaseController
	{
		Dictionary<Deployment, Subcontroller> _DeploymentMicrocontrollers = new Dictionary<Deployment, Subcontroller>();

		DeploymentPane _DeploymentPane;
		Deployment _WorkingDeployment;

		UnitConfigurationRenderer _Renderer;

		public DeploymentController(Match Match, UnitConfigurationRenderer Renderer, GameScreen GameScreen)
			: base(Match, GameScreen)
		{
			_Renderer = Renderer;
		}

		public override void Begin(Army Army)
		{
			base.Begin(Army);
			_DeploymentPane = new DeploymentPane();
			_DeploymentPane.OnDeploymentSelected += HandleDeploymentSelected;
			_DeploymentMicrocontrollers.Clear();
			foreach (Deployment d in Army.Deployments)
			{
				DeploymentMicrocontroller c;
				if (d is PositionalDeployment)
					c = new PositionalDeploymentMicrocontroller(
						_Match, _GameScreen, (PositionalDeployment)d, _Renderer);
				else c = new ConvoyDeploymentMicrocontroller(_Match, _GameScreen, (ConvoyDeployment)d, _Renderer);
				_DeploymentMicrocontrollers.Add(d, c);
				_DeploymentPane.AddPage(c.DeploymentPage);
			}
			_GameScreen.AddPane(_DeploymentPane);
		}

		public override void End()
		{
			base.End();
			_GameScreen.RemovePane(_DeploymentPane);
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			_DeploymentMicrocontrollers[_DeploymentPane.SelectedDeployment].HandleTileLeftClick(Tile);
		}

		public override void HandleTileRightClick(Tile Tile)
		{
			_DeploymentMicrocontrollers[_DeploymentPane.SelectedDeployment].HandleTileRightClick(Tile);
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			_DeploymentMicrocontrollers[Unit.Deployment].HandleUnitLeftClick(Unit);
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
			_DeploymentMicrocontrollers[Unit.Deployment].HandleUnitRightClick(Unit);
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
		}

		void HandleDeploymentSelected(object Sender, EventArgs E)
		{
			if (_WorkingDeployment != null) _DeploymentMicrocontrollers[_WorkingDeployment].End();
			_WorkingDeployment = _DeploymentPane.SelectedDeployment;
			_DeploymentMicrocontrollers[_WorkingDeployment].Begin(_Army);
		}
	}
}
