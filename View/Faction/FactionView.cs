using System;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class FactionView : Pod
	{
		public Vector2f Position;

		public readonly Faction Faction;
		public readonly float Scale;

		Texture _Texture;
		Vertex[] _Vertices;

		public FactionView(Faction Faction, float Scale)
		{
			this.Faction = Faction;
			this.Scale = Scale;

			FactionRenderDetails renderDetails = GameData.FactionRenderDetails[Faction.UniqueKey];
			_Texture = new Texture(renderDetails.ImagePath);
			_Vertices = new Vertex[]
			{
				new Vertex(new Vector2f(0, 0), Color.White, new Vector2f(0, 0)),
				new Vertex(new Vector2f(1, 0), Color.White, new Vector2f(_Texture.Size.X, 0)),
				new Vertex(new Vector2f(1, 1), Color.White, new Vector2f(_Texture.Size.X, _Texture.Size.Y)),
				new Vertex(new Vector2f(0, 1), Color.White, new Vector2f(0, _Texture.Size.Y))
			};
		}

		public void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			Transform.Scale(Scale, Scale);
			RenderStates r = new RenderStates(_Texture);
			r.Transform = Transform;
			Target.Draw(_Vertices, PrimitiveType.Quads, r);
		}
	}
}
