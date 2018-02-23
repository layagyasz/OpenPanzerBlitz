using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class FactionRenderer : SquareSpriteSheetRenderer<Faction>
	{
		public readonly Dictionary<string, FactionRenderDetails> RenderDetails;

		public FactionRenderer(
			IEnumerable<Faction> Factions,
			Dictionary<string, FactionRenderDetails> RenderDetails,
			uint SpriteSize,
			uint TextureSize)
		{
			this.RenderDetails = RenderDetails;

			RenderAll(Factions, SpriteSize, TextureSize);
		}

		public FactionRenderer(
			Scenario Scenario,
			Dictionary<string, FactionRenderDetails> RenderDetails,
			uint SpriteSize,
			uint TextureSize)
			: this(
				Scenario.ArmyConfigurations.Select(i => i.Faction).Distinct(),
				RenderDetails,
				SpriteSize,
				TextureSize)
		{ }

		public override void Render(RenderTarget Target, Transform Transform, Faction Object)
		{
			var image = new Sprite(new Texture(RenderDetails[Object.UniqueKey].ImagePath));
			image.Draw(Target, new RenderStates(Transform));
		}
	}
}
