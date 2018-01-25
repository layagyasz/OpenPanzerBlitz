using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitConfigurationSelectionOption : Button
	{
		public readonly UnitConfigurationLink UnitConfigurationLink;
		public readonly UnitConfigurationStackView StackView;

		Button _PointText;

		public UnitConfigurationSelectionOption(
			string ClassName,
			string DetailsClassName,
			string OverlayClassName,
			UnitConfigurationLink UnitConfigurationLink,
			Faction Faction,
			UnitConfigurationRenderer Renderer,
			bool DisplayCount)
			: base(ClassName)
		{
			this.UnitConfigurationLink = UnitConfigurationLink;

			StackView = new UnitConfigurationStackView(
				UnitConfigurationLink.UnitConfiguration,
				Faction,
				Renderer,
				Class.GetAttributeWithDefault("unit-scale", 0),
				OverlayClassName,
				DisplayCount);
			StackView.Position = .5f * StackView.Size + new Vector2f(12, 12);
			StackView.Parent = this;
			Value = StackView;

			_PointText = new Button(DetailsClassName);
			_PointText.Position = new Vector2f(12, StackView.Size.Y + 16);
			_PointText.Parent = this;
			_PointText.DisplayedString = UnitConfigurationLink.UnitConfiguration.GetPointValue().ToString();
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);

			StackView.Update(MouseController, KeyController, DeltaT, Transform);
			_PointText.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);

			StackView.Draw(Target, Transform);
			_PointText.Draw(Target, Transform);
		}
	}
}
