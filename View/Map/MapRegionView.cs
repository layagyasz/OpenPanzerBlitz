using System;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MapRegionView : Pod
	{
		public readonly MapRegion MapRegion;

		Text _Text;

		public MapRegionView(MapRegion MapRegion, TileRenderer Renderer)
		{
			this.MapRegion = MapRegion;
			_Text = new Text(MapRegion.Name, Renderer.FontFace, 32);
			_Text.Color = Renderer.FontColor;
			_Text.Scale = new Vector2f(1f / 128, 1f / 128);

			MapRegion.OnChange += (sender, e) => Update();
			Update();
		}

		void Update()
		{
			_Text.DisplayedString = MapRegion.Name;

			Vector2f center = GetCenter(_Text);
			if (MapRegion.Tiles.Count() > 0)
			{
				Vector2f tl = new Vector2f(MapRegion.Tiles.Min(i => i.Center.X), MapRegion.Tiles.Min(i => i.Center.Y));
				Vector2f br = new Vector2f(MapRegion.Tiles.Max(i => i.Center.X), MapRegion.Tiles.Max(i => i.Center.Y));
				_Text.Position = .5f * (tl + br) - center;
			}
			else _Text.Position = -center;
		}

		Vector2f GetCenter(Text Text)
		{
			return new Vector2f(
				Text.GetLocalBounds().Width * Text.Scale.X, Text.GetLocalBounds().Height * Text.Scale.Y) * .5f;
		}

		public void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			Target.Draw(_Text, new RenderStates(Transform));
		}
	}
}
