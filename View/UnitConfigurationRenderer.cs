using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class UnitConfigurationRenderer
	{
		public readonly Font Font;
		public readonly uint SpriteSize;
		public readonly uint TextureSize;

		List<Texture> _Textures = new List<Texture>();
		Dictionary<UnitConfiguration, Tuple<Texture, Vector2f[]>> _RenderInfo =
			new Dictionary<UnitConfiguration, Tuple<Texture, Vector2f[]>>();

		public UnitConfigurationRenderer(Scenario Scenario, uint TextureSize, uint SpriteSize, Font Font)
		{
			this.TextureSize = TextureSize;
			this.SpriteSize = SpriteSize;
			this.Font = Font;
			RenderAll(
				Scenario.ArmyConfigurations
				.SelectMany(i => i.DeploymentConfigurations)
				.SelectMany(i => i.UnitConfigurations).Distinct());
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
					_Textures.Add(renderedTexture);
					texture = new RenderTexture(TextureSize, TextureSize);
				}
			}
			texture.Display();
			renderedTexture = new Texture(texture.Texture);
			renderedTexture.CopyToImage().SaveToFile("out.png");
			foreach (KeyValuePair<UnitConfiguration, Vector2f[]> k in renderInfoCache)
				_RenderInfo.Add(k.Key, new Tuple<Texture, Vector2f[]>(renderedTexture, k.Value));
			_Textures.Add(renderedTexture);
		}

		private void Render(RenderTarget Target, Transform Transform, UnitConfiguration UnitConfiguration)
		{

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

			Text weaponClassText = new Text(WeaponClassString(UnitConfiguration), Font, 36);
			weaponClassText.Color = Color.Black;
			weaponClassText.Position = SpriteSize * new Vector2f(.5f, 1f / 12) - GetCenter(weaponClassText);

			Text nameText = new Text(
				UnitConfiguration.VerticalSplitImage ?
				UnitConfiguration.Name.Replace(" ", "   ") : UnitConfiguration.Name, Font, 18);
			nameText.Color = Color.Black;
			if (UnitConfiguration.VerticalSplitImage)
				nameText.Position = SpriteSize * new Vector2f(.5f, 7f / 12) - GetCenter(nameText);
			else nameText.Position = SpriteSize * new Vector2f(.5f, 3f / 4) - GetCenter(nameText);

			Sprite image = new Sprite(new Texture("./UnitSprites/" + UnitConfiguration.ImageName));

			RenderStates r = new RenderStates(Transform);
			Target.Draw(image, r);
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
			if (UnitConfiguration.IsCarrier)
			{
				if (UnitConfiguration.WeaponClass == WeaponClass.NA) return "C";
				else return string.Format("C({0})", UnitConfiguration.WeaponClass.ToString()[0]);
			}
			else return UnitConfiguration.WeaponClass.ToString().Substring(0, 1);
		}
	}
}
