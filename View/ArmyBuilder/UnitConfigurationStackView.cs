using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitConfigurationStackView : Interactive
	{
		bool _DisplayCount;
		UnitConfigurationView _UnitConfigurationView;
		Button _Text;

		public int Count { get; set; } = 1;

		public override Vector2f Size
		{
			get
			{
				return _UnitConfigurationView.Size;
			}
		}

		public UnitConfigurationStackView(
			UnitConfiguration UnitConfiguration,
			Faction Faction,
			UnitConfigurationRenderer Renderer,
			int UnitScale,
			string OverlayClassName,
			bool DisplayCount)
		{
			_DisplayCount = DisplayCount;
			_UnitConfigurationView = new UnitConfigurationView(UnitConfiguration, Faction, Renderer, UnitScale);
			if (_DisplayCount)
			{
				_Text = new Button(OverlayClassName);
				_Text.Position = -.5f * new Vector2f(UnitScale, _Text.Size.Y);
				_Text.Parent = this;
			}
		}

		public override bool IsCollision(Vector2f Point)
		{
			return _UnitConfigurationView.IsCollision(Point);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			Transform.Translate(Position);
			if (_DisplayCount)
			{
				_Text.DisplayedString = "x" + Count;
				_Text.Update(MouseController, KeyController, DeltaT, Transform);
			}
			_UnitConfigurationView.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			_UnitConfigurationView.Draw(Target, Transform);
			if (_DisplayCount) _Text.Draw(Target, Transform);
		}
	}
}
