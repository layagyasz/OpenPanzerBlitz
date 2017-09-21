using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitConfigurationRenderer
	{
		public readonly Dictionary<UnitConfiguration, UnitRenderDetails> RenderDetails;
		public readonly Font Font;
		public readonly uint SpriteSize;
		public readonly uint TextureSize;

		List<Texture> _Textures = new List<Texture>();
		Dictionary<UnitConfiguration, Tuple<Texture, Vector2f[]>> _RenderInfo =
			new Dictionary<UnitConfiguration, Tuple<Texture, Vector2f[]>>();

		public UnitConfigurationRenderer(
			Scenario Scenario,
			Dictionary<UnitConfiguration, UnitRenderDetails> RenderDetails,
			uint TextureSize,
			uint SpriteSize,
			Font Font)
		{
			this.RenderDetails = RenderDetails;
			this.TextureSize = TextureSize;
			this.SpriteSize = SpriteSize;
			this.Font = Font;
			RenderAll(
				new UnitConfiguration[] { GameData.Wreckage }
				.Concat(Scenario.ArmyConfigurations
					.SelectMany(i => i.DeploymentConfigurations)
						.SelectMany(i => i.UnitGroup.UnitConfigurations).Distinct()));
		}

		public Tuple<Texture, Vector2f[]> GetRenderInfo(UnitConfiguration UnitConfiguration)
		{
			return _RenderInfo[UnitConfiguration];
		}

		private void RenderAll(IEnumerable<UnitConfiguration> UnitConfigurations)
		{
			RenderTexture texture = new RenderTexture(TextureSize, TextureSize);
			Texture renderedTexture;

			List<KeyValuePair<UnitConfiguration, Vector2f[]>> renderInfoCache =
				new List<KeyValuePair<UnitConfiguration, Vector2f[]>>();
			int i = 0;
			int j = 0;
			uint rowSprites = TextureSize / SpriteSize;
			foreach (UnitConfiguration u in UnitConfigurations)
			{
				Transform t = Transform.Identity;
				t.Translate(new Vector2f(i * SpriteSize, j * SpriteSize));
				Render(texture, t, u);
				renderInfoCache.Add(new KeyValuePair<UnitConfiguration, Vector2f[]>(u, new Vector2f[]
				{
					new Vector2f(SpriteSize * i, SpriteSize * j),
					new Vector2f(SpriteSize* (i + 1), SpriteSize * j),
					new Vector2f(SpriteSize* (i + 1), SpriteSize * (j + 1)),
					new Vector2f(SpriteSize * i, SpriteSize * (j + 1))
				}));

				i++;
				if (i >= rowSprites)
				{
					i = 0;
					j++;
				}
				if (j >= rowSprites)
				{
					texture.Display();
					renderedTexture = new Texture(texture.Texture);
					foreach (KeyValuePair<UnitConfiguration, Vector2f[]> k in renderInfoCache)
						_RenderInfo.Add(k.Key, new Tuple<Texture, Vector2f[]>(renderedTexture, k.Value));
					_RenderInfo.Clear();
					_Textures.Add(renderedTexture);
					texture = new RenderTexture(TextureSize, TextureSize);
					i = 0;
					j = 0;
				}
			}
			texture.Display();
			renderedTexture = new Texture(texture.Texture);
			foreach (KeyValuePair<UnitConfiguration, Vector2f[]> k in renderInfoCache)
				_RenderInfo.Add(k.Key, new Tuple<Texture, Vector2f[]>(renderedTexture, k.Value));
			_Textures.Add(renderedTexture);
		}

		private void Render(RenderTarget Target, Transform Transform, UnitConfiguration UnitConfiguration)
		{
			UnitRenderDetails renderDetails = RenderDetails[UnitConfiguration];

			Sprite image = new Sprite(new Texture("./UnitSprites/" + renderDetails.ImagePath));
			RenderStates r = new RenderStates(Transform);
			Target.Draw(image, r);

			// Do not render stats for blocks and mines.
			if (UnitConfiguration.UnitClass == UnitClass.BLOCK
				|| UnitConfiguration.UnitClass == UnitClass.MINEFIELD
				|| UnitConfiguration.UnitClass == UnitClass.WRECKAGE)
				return;

			// Only render defense for forts.
			if (UnitConfiguration.UnitClass == UnitClass.FORT)
			{
				Text fortText = new Text(UnitConfiguration.Defense.ToString(), Font, 56);
				fortText.Color = Color.Black;
				fortText.Position = SpriteSize * new Vector2f(1f / 2, 1f / 3) - GetCenter(fortText);
				Target.Draw(fortText, r);
				return;
			}

			Text attackText = new Text(UnitConfiguration.Attack.ToString(), Font, 36);
			attackText.Color = Color.Black;
			attackText.Position = SpriteSize * new Vector2f(1f / 6, 1f / 12) - GetCenter(attackText);

			Text rangeText = new Text(UnitConfiguration.Range.ToString(), Font, 36);
			rangeText.Color = Color.Black;
			rangeText.Position = SpriteSize * new Vector2f(5f / 6, 1f / 12) - GetCenter(rangeText);

			Text defenseText = new Text(UnitConfiguration.Defense.ToString(), Font, 36);
			defenseText.Color = Color.Black;
			defenseText.Position = SpriteSize * new Vector2f(1f / 6, 3f / 4) - GetCenter(defenseText);

			Text moveText = new Text(UnitConfiguration.Movement.ToString(), Font, 36);
			moveText.Color = Color.Black;
			moveText.Position = SpriteSize * new Vector2f(5f / 6, 3f / 4) - GetCenter(moveText);

			Text weaponClassText = new Text(WeaponClassString(UnitConfiguration), Font, 28);
			weaponClassText.Color = Color.Black;
			weaponClassText.Position = SpriteSize * new Vector2f(.5f, 1f / 12) - GetCenter(weaponClassText);

			Text nameText = new Text(
				renderDetails.OverrideDisplayName == null ? UnitConfiguration.Name : renderDetails.OverrideDisplayName,
				Font,
				24);
			nameText.Color = Color.Black;
			nameText.Position = SpriteSize * new Vector2f(.5f, 13f / 16) - GetCenter(nameText);

			Target.Draw(attackText, r);
			Target.Draw(rangeText, r);
			Target.Draw(defenseText, r);
			Target.Draw(moveText, r);
			Target.Draw(weaponClassText, r);
			Target.Draw(nameText, r);
		}

		private Vector2f GetCenter(Text Text)
		{
			return new Vector2f(Text.GetLocalBounds().Width, Text.GetLocalBounds().Height) * .5f;
		}

		private static string WeaponClassString(UnitConfiguration UnitConfiguration)
		{
			if (UnitConfiguration.UnitClass == UnitClass.TRANSPORT)
			{
				if (UnitConfiguration.WeaponClass == WeaponClass.NA) return "C";
				else return string.Format("C({0})", UnitConfiguration.WeaponClass.ToString()[0]);
			}
			if (UnitConfiguration.WeaponClass == WeaponClass.NA) return "-";
			if (UnitConfiguration.CanIndirectFire && UnitConfiguration.WeaponClass == WeaponClass.HIGH_EXPLOSIVE)
				return string.Format("({0})", UnitConfiguration.WeaponClass.ToString().Substring(0, 1));
			return UnitConfiguration.WeaponClass.ToString().Substring(0, 1);
		}
	}
}
