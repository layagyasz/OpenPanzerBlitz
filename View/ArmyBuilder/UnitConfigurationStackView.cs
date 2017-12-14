using System;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitConfigurationStackView : Interactive
	{
		static readonly int UNIT_VIEW_SCALE = 64;
		static readonly uint FONT_SIZE = 18;

		bool _DisplayCount;
		UnitConfigurationView _UnitConfigurationView;
		Text _Text;

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
			Font Font,
			bool DisplayCount)
		{
			_DisplayCount = DisplayCount;
			_UnitConfigurationView = new UnitConfigurationView(UnitConfiguration, Faction, Renderer, UNIT_VIEW_SCALE);
			if (_DisplayCount) _Text = new Text("", Font, FONT_SIZE) { Color = Color.Red };
		}

		public override bool IsCollision(Vector2f Point)
		{
			return _UnitConfigurationView.IsCollision(Point);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			Transform.Translate(Position);
			if (_DisplayCount) _Text.DisplayedString = "x" + Count;
			_UnitConfigurationView.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			_UnitConfigurationView.Draw(Target, Transform);
			if (_DisplayCount)
			{
				_Text.Position = -.5f * new Vector2f(_Text.GetLocalBounds().Width, _Text.GetLocalBounds().Height);
				_Text.Draw(Target, new RenderStates(Transform));
			}
		}
	}
}
