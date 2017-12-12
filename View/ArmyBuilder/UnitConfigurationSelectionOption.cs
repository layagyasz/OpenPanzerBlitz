using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class UnitConfigurationSelectionOption : Button
	{
		UnitConfigurationStackView _StackView;

		public UnitConfigurationSelectionOption(
			string ClassName,
			UnitConfiguration UnitConfiguration,
			Faction Faction,
			UnitConfigurationRenderer Renderer)
			: base(ClassName)
		{
			_StackView = new UnitConfigurationStackView(
				UnitConfiguration, Faction, Renderer, Class.GetAttributeWithDefault<Font>("unit-stack-font", null));
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
