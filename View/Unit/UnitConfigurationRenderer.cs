using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitConfigurationRenderer : SquareSpriteSheetRenderer<UnitConfiguration>
	{
		public readonly Dictionary<string, UnitRenderDetails> RenderDetails;
		public readonly Font Font;
		public readonly uint SpriteSize;

		List<Texture> _Textures = new List<Texture>();
		Dictionary<UnitConfiguration, Tuple<Texture, Vector2f[]>> _RenderInfo =
			new Dictionary<UnitConfiguration, Tuple<Texture, Vector2f[]>>();

		public UnitConfigurationRenderer(
			IEnumerable<UnitConfiguration> UnitConfigurations,
			Dictionary<string, UnitRenderDetails> RenderDetails,
			uint SpriteSize,
			uint TextureSize,
			Font Font)
		{
			this.RenderDetails = RenderDetails;
			this.Font = Font;
			this.SpriteSize = SpriteSize;

			RenderAll(UnitConfigurations, SpriteSize, TextureSize);
		}

		public UnitConfigurationRenderer(
			Scenario Scenario,
			Dictionary<string, UnitRenderDetails> RenderDetails,
			uint SpriteSize,
			uint TextureSize,
			Font Font)
			: this(
				new UnitConfiguration[] { GameData.Wreckage }.Concat(Scenario.UnitConfigurations.Distinct()),
  	 			RenderDetails,
				SpriteSize,
				TextureSize,
				Font)
		{ }

		public override void Render(RenderTarget Target, Transform Transform, UnitConfiguration UnitConfiguration)
		{
			UnitRenderDetails renderDetails = RenderDetails[UnitConfiguration.UniqueKey];

			var image = new Sprite(new Texture(renderDetails.ImagePath));
			var r = new RenderStates(Transform);
			Target.Draw(image, r);

			// Do not render stats for blocks and mines.
			if (UnitConfiguration.UnitClass == UnitClass.BLOCK
				|| UnitConfiguration.UnitClass == UnitClass.MINEFIELD
				|| UnitConfiguration.UnitClass == UnitClass.WRECKAGE)
				return;

			// Only render defense for forts.
			if (UnitConfiguration.UnitClass == UnitClass.FORT)
			{
				var fortText = new Text(UnitConfiguration.Defense.ToString(), Font, 56);
				fortText.Color = Color.Black;
				fortText.Position = SpriteSize * new Vector2f(1f / 2, 1f / 3) - GetCenter(fortText);
				Target.Draw(fortText, r);
				return;
			}

			var attackText = new Text(UnitConfiguration.Attack.ToString(), Font, 36);
			attackText.Color = Color.Black;
			attackText.Position = SpriteSize * new Vector2f(1f / 6, 1f / 12) - GetCenter(attackText);

			var rangeText = new Text(
				UnitConfiguration.Range.ToString() + (UnitConfiguration.CanDoubleRange ? "*" : ""), Font, 36);
			rangeText.Color = Color.Black;
			rangeText.Position = SpriteSize * new Vector2f(5f / 6, 1f / 12) - GetCenter(rangeText);

			var defenseText = new Text(UnitConfiguration.Defense.ToString(), Font, 36);
			defenseText.Color = Color.Black;
			defenseText.Position = SpriteSize * new Vector2f(1f / 6, 3f / 4) - GetCenter(defenseText);

			var moveText = new Text(
				UnitConfiguration.Movement
				+ (UnitConfiguration.MovementRules.Water.BlockType == BlockType.IMPASSABLE ? "" : "*"), Font, 36);
			moveText.Color = Color.Black;
			moveText.Position = SpriteSize * new Vector2f(5f / 6, 3f / 4) - GetCenter(moveText);

			var weaponClassText = new Text(WeaponClassString(UnitConfiguration), Font, 28);
			weaponClassText.Color = Color.Black;
			weaponClassText.Position = SpriteSize * new Vector2f(.5f, 1f / 12) - GetCenter(weaponClassText);

			var nameText = new Text(renderDetails.OverrideDisplayName ?? UnitConfiguration.Name, Font, 24);
			nameText.Color = Color.Black;
			nameText.Position = SpriteSize * new Vector2f(.5f, 13f / 16) - GetCenter(nameText);

			Target.Draw(attackText, r);
			Target.Draw(rangeText, r);
			Target.Draw(defenseText, r);
			Target.Draw(moveText, r);
			Target.Draw(weaponClassText, r);
			Target.Draw(nameText, r);
		}

		Vector2f GetCenter(Text Text)
		{
			return new Vector2f(Text.GetLocalBounds().Width, Text.GetLocalBounds().Height) * .5f;
		}

		static string WeaponClassString(UnitConfiguration UnitConfiguration)
		{
			if (UnitConfiguration.IsCarrier && !UnitConfiguration.CanOnlyCarryInfantry)
			{
				if (UnitConfiguration.WeaponClass == WeaponClass.NA) return "C";
				return string.Format("C({0})", UnitConfiguration.WeaponClass.ToString()[0]);
			}
			if (UnitConfiguration.WeaponClass == WeaponClass.NA) return "-";
			if (UnitConfiguration.CanIndirectFire && UnitConfiguration.WeaponClass == WeaponClass.HIGH_EXPLOSIVE)
				return string.Format("({0})", UnitConfiguration.WeaponClass.ToString().Substring(0, 1));
			if (UnitConfiguration.CanAntiAircraft)
				return string.Format("<{0}>", UnitConfiguration.WeaponClass.ToString().Substring(0, 1));
			return UnitConfiguration.WeaponClass.ToString().Substring(0, 1);
		}
	}
}
