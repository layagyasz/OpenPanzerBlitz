using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class SingularUnitSelectionOption : SelectionOption<StackView>
	{
		static readonly int UNIT_VIEW_SCALE = 64;

		public readonly Unit Unit;

		StackView _StackView = new StackView();

		public SingularUnitSelectionOption(string ClassName, Unit Unit, UnitConfigurationRenderer Renderer)
					: base(ClassName)
		{
			this.Unit = Unit;

			_StackView.Add(new UnitView(Unit, Renderer, UNIT_VIEW_SCALE));
			_StackView.Position = Size / 2;
			_StackView.Parent = this;
			Value = _StackView;
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);

			_StackView.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);

			_StackView.Draw(Target, Transform);
		}
	}
}
