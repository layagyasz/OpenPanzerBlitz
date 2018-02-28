using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Window;

namespace PanzerBlitz
{
	public class DeploymentController : BaseController
	{
		readonly Dictionary<Deployment, Subcontroller> _DeploymentMicrocontrollers =
			new Dictionary<Deployment, Subcontroller>();

		DeploymentPane _DeploymentPane;
		Deployment _WorkingDeployment;

		public DeploymentController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void Begin()
		{
			base.Begin();

			_DeploymentPane = new DeploymentPane();
			_DeploymentPane.OnDeploymentSelected += HandleDeploymentSelected;
			_DeploymentMicrocontrollers.Clear();
			foreach (Deployment d in _Controller.CurrentTurn.Army.Deployments.Where(i => !i.IsConfigured()))
			{
				DeploymentMicrocontroller c;
				if (d is PositionalDeployment)
					c = new PositionalDeploymentMicrocontroller(
						_Controller, (PositionalDeployment)d);
				else c = new ConvoyDeploymentMicrocontroller(_Controller, (ConvoyDeployment)d);
				_DeploymentMicrocontrollers.Add(d, c);
				_DeploymentPane.AddPage(c.MakePage(_DeploymentPane, _Controller.UnitConfigurationRenderer));
			}
			_Controller.AddPane(_DeploymentPane);
		}

		public override bool Finish()
		{
			return _DeploymentMicrocontrollers.All(i => i.Value.Finish());
		}

		public override void End()
		{
			base.End();
			if (_WorkingDeployment != null) _DeploymentMicrocontrollers[_WorkingDeployment].End();
			_WorkingDeployment = null;
		}

		public override bool CanLoad()
		{
			return _DeploymentMicrocontrollers[_WorkingDeployment].CanLoad();
		}

		public override bool CanUnload()
		{
			return _DeploymentMicrocontrollers[_WorkingDeployment].CanUnload();
		}

		public override bool CanDismount()
		{
			return _DeploymentMicrocontrollers[_WorkingDeployment].CanDismount();
		}

		public override bool CanMount()
		{
			return _DeploymentMicrocontrollers[_WorkingDeployment].CanMount();
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
			if (_DeploymentMicrocontrollers.ContainsKey(Unit.Deployment))
				_DeploymentMicrocontrollers[Unit.Deployment].HandleUnitLeftClick(Unit);
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
			if (_DeploymentMicrocontrollers.ContainsKey(Unit.Deployment))
				_DeploymentMicrocontrollers[Unit.Deployment].HandleUnitRightClick(Unit);
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
			_DeploymentMicrocontrollers[_DeploymentPane.SelectedDeployment].HandleKeyPress(Key);
		}

		void HandleDeploymentSelected(object Sender, EventArgs E)
		{
			if (_WorkingDeployment != null) _DeploymentMicrocontrollers[_WorkingDeployment].End();
			_WorkingDeployment = _DeploymentPane.SelectedDeployment;
			_DeploymentMicrocontrollers[_WorkingDeployment].Begin();
		}
	}
}
