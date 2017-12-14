using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class UnitConfigurationSelectionOption : Button
	{
		public readonly UnitConfiguration UnitConfiguration;

		public UnitConfigurationStackView StackView;

		public UnitConfigurationSelectionOption(
			string ClassName,
			UnitConfiguration UnitConfiguration,
			Faction Faction,
			UnitConfigurationRenderer Renderer,
			bool DisplayCount)
			: base(ClassName)
		{
			this.UnitConfiguration = UnitConfiguration;

			StackView = new UnitConfigurationStackView(
				UnitConfiguration,
				Faction,
				Renderer,
				Class.GetAttributeWithDefault<Font>("unit-stack-font", null),
				DisplayCount);
			StackView.Position = Size / 2;
			StackView.Parent = this;
			Value = StackView;
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);

			StackView.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);

			StackView.Draw(Target, Transform);
		}
	}
}
