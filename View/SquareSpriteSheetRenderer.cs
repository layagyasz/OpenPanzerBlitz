using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public abstract class SquareSpriteSheetRenderer<T>
	{
		List<Texture> _Textures = new List<Texture>();
		Dictionary<T, Tuple<Texture, Vector2f[]>> _RenderInfo = new Dictionary<T, Tuple<Texture, Vector2f[]>>();

		protected void RenderAll(IEnumerable<T> Objects, uint SpriteSize, uint TextureSize)
		{
			RenderTexture texture = new RenderTexture(TextureSize, TextureSize);
			Texture renderedTexture;

			List<KeyValuePair<T, Vector2f[]>> renderInfoCache = new List<KeyValuePair<T, Vector2f[]>>();
			int i = 0;
			int j = 0;
			uint rowSprites = TextureSize / SpriteSize;
			foreach (T o in Objects)
			{
				Transform t = Transform.Identity;
				t.Translate(new Vector2f(i * SpriteSize, j * SpriteSize));
				Render(texture, t, o);
				renderInfoCache.Add(new KeyValuePair<T, Vector2f[]>(o, new Vector2f[]
				{
					new Vector2f(SpriteSize * i, SpriteSize * j),
					new Vector2f(SpriteSize * (i + 1), SpriteSize * j),
					new Vector2f(SpriteSize * (i + 1), SpriteSize * (j + 1)),
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
					DumpCache(renderInfoCache, renderedTexture);
					_Textures.Add(renderedTexture);
					texture = new RenderTexture(TextureSize, TextureSize);
					i = 0;
					j = 0;
				}
			}
			texture.Display();
			renderedTexture = new Texture(texture.Texture);
			DumpCache(renderInfoCache, renderedTexture);
			_Textures.Add(renderedTexture);
		}

		void DumpCache(List<KeyValuePair<T, Vector2f[]>> Cache, Texture Texture)
		{
			foreach (KeyValuePair<T, Vector2f[]> k in Cache)
			{
				try
				{
					_RenderInfo.Add(k.Key, new Tuple<Texture, Vector2f[]>(Texture, k.Value));
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("Error Caching {0}: {1}", k.Key, e));
				}
			}
			Cache.Clear();
		}

		public Tuple<Texture, Vector2f[]> GetRenderInfo(T Object)
		{
			return _RenderInfo[Object];		}

		public abstract void Render(RenderTarget Target, Transform Transform, T Object);
	}
}
