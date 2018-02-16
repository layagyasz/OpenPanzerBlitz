using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class FactionView : GuiItem
	{
		public readonly Faction Faction;
		public readonly float Scale;

		readonly Texture _Texture;
		readonly Vertex[] _Vertices;

		public override Vector2f Size
		{
			get
			{
				return new Vector2f(Scale, Scale);
			}
		}

		public FactionView(Faction Faction, FactionRenderer Renderer, float Scale)
		{
			this.Faction = Faction;
			this.Scale = Scale;

			var renderInfo = Renderer.GetRenderInfo(Faction);
			_Texture = renderInfo.Item1;
			_Vertices = new Vertex[]
			{
				new Vertex(new Vector2f(0, 0), Color.White, renderInfo.Item2[0]),
				new Vertex(new Vector2f(1, 0), Color.White, renderInfo.Item2[1]),
				new Vertex(new Vector2f(1, 1), Color.White, renderInfo.Item2[2]),
				new Vertex(new Vector2f(0, 1), Color.White, renderInfo.Item2[3])
			};
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			Transform.Scale(Scale, Scale);
			var r = new RenderStates(_Texture);
			r.Transform = Transform;
			Target.Draw(_Vertices, PrimitiveType.Quads, r);
		}
	}
}
