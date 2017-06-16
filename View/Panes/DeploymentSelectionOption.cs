using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class DeploymentSelectionOption : SelectionOption<Unit>
	{
		UnitView _UnitView;

		public DeploymentSelectionOption(Unit Unit)
			: base("deployment-selection-option")
		{
			_UnitView = new UnitView(Unit, 64);
			_UnitView.Position = Size / 2;
			_UnitView.Parent = this;
			Value = _UnitView.Unit;
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);

			_UnitView.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);

			_UnitView.Draw(Target, Transform);
		}
	}
}
