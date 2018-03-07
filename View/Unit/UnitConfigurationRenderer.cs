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

		public override void Render(RenderTarget Target, Transform Transform, UnitConfiguration Object)
		{
			UnitRenderDetails renderDetails = RenderDetails[Object.UniqueKey];

			var image = new Sprite(new Texture(renderDetails.ImagePath));
			var r = new RenderStates(Transform);
			Target.Draw(image, r);

			// Do not render stats for blocks and mines.
			if (Object.UnitClass == UnitClass.BLOCK
				|| Object.UnitClass == UnitClass.MINEFIELD
				|| Object.UnitClass == UnitClass.WRECKAGE)
				return;

			// Only render defense for forts.
			if (Object.UnitClass == UnitClass.FORT)
			{
				var fortText = new Text(Object.Defense.ToString(), Font, 56);
				fortText.Color = Color.Black;
				fortText.Position = SpriteSize * new Vector2f(1f / 2, 1f / 3) - GetCenter(fortText);
				Target.Draw(fortText, r);
				return;
			}

			if (Object.SecondaryWeapon == default(Weapon)) RenderWeapon(Target, r, Object, Object.GetWeapon(false));
			else
			{
				RenderWeapon(Target, r, Object, Object.GetWeapon(false), new Vector2f(1f / 6, 1f / 12), true);
				RenderWeapon(Target, r, Object, Object.GetWeapon(true), new Vector2f(5f / 6, 1f / 12), true);
			}

			var defenseText = new Text(Object.Defense.ToString(), Font, 36);
			defenseText.Color = Color.Black;
			defenseText.Position = SpriteSize * new Vector2f(1f / 6, 3f / 4) - GetCenter(defenseText);

			var moveText = new Text(
				Object.Movement
				+ (Object.MovementRules.Water.BlockType == BlockType.IMPASSABLE ? "" : "*"), Font, 36);
			moveText.Color = Color.Black;
			moveText.Position = SpriteSize * new Vector2f(5f / 6, 3f / 4) - GetCenter(moveText);

			var nameText = new Text(renderDetails.OverrideDisplayName ?? Object.Name, Font, 24);
			nameText.Color = Color.Black;
			nameText.Position = SpriteSize * new Vector2f(.5f, 13f / 16) - GetCenter(nameText);

			if (!Object.HasUnlimitedMovement()) Target.Draw(moveText, r);
			Target.Draw(defenseText, r);
			Target.Draw(nameText, r);
		}

		void RenderWeapon(
			RenderTarget Target,
			RenderStates RenderStates,
			UnitConfiguration Object,
			Weapon Weapon,
			Vector2f Origin = default(Vector2f),
			bool Vertical = false)
		{
			var attackText = new Text(Weapon.Attack.ToString(), Font, 36) { Color = Color.Black };
			var weaponClassText = new Text(WeaponClassString(Object, false), Font, 28) { Color = Color.Black };
			var rangeText = new Text(
				Weapon.Range.ToString() + (Weapon.CanDoubleRange ? "*" : ""), Font, 36)
			{ Color = Color.Black };
			if (Vertical)
			{
				Vector2f padding = new Vector2f(0, 1f / 4);
				attackText.Position = SpriteSize * Origin - GetCenter(attackText);
				weaponClassText.Position = SpriteSize * (Origin + padding * 2) - GetCenter(weaponClassText);
				rangeText.Position = SpriteSize * (Origin + padding * 1) - GetCenter(rangeText);
			}
			else
			{
				attackText.Position = SpriteSize * new Vector2f(1f / 6, 1f / 12) - GetCenter(attackText);
				weaponClassText.Position = SpriteSize * new Vector2f(.5f, 1f / 12) - GetCenter(weaponClassText);
				rangeText.Position = SpriteSize * new Vector2f(5f / 6, 1f / 12) - GetCenter(rangeText);
			}
			Target.Draw(attackText, RenderStates);
			Target.Draw(weaponClassText, RenderStates);
			Target.Draw(rangeText, RenderStates);
		}

		Vector2f GetCenter(Text Text)
		{
			return new Vector2f(Text.GetLocalBounds().Width, Text.GetLocalBounds().Height) * .5f;
		}

		static string WeaponClassString(UnitConfiguration UnitConfiguration, bool Secondary)
		{
			Weapon weapon = UnitConfiguration.GetWeapon(Secondary);
			if (UnitConfiguration.IsCarrier && !UnitConfiguration.CanOnlyCarryInfantry)
			{
				if (weapon.WeaponClass == WeaponClass.NA) return "C";
				return string.Format("C({0})", weapon.WeaponClass.ToString()[0]);
			}
			if (weapon.WeaponClass == WeaponClass.NA) return "-";
			if (UnitConfiguration.CanIndirectFire && weapon.WeaponClass == WeaponClass.HIGH_EXPLOSIVE)
				return string.Format("({0})", weapon.WeaponClass.ToString().Substring(0, 1));
			if (UnitConfiguration.CanAntiAircraft)
				return string.Format("<{0}>", weapon.WeaponClass.ToString().Substring(0, 1));
			return weapon.WeaponClass.ToString().Substring(0, 1);
		}
	}
}
