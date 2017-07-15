using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class HomogenousStackView : Interactive
	{
		static readonly int UNIT_VIEW_SCALE = 64;
		static readonly uint FONT_SIZE = 24;

		Text _Text;
		UnitConfigurationRenderer _Renderer;
		Stack<UnitView> _UnitViews;

		public override Vector2f Size
		{
			get
			{
				return _UnitViews.First().Size;
			}
		}

		public int Count
		{
			get
			{
				return _UnitViews.Count;
			}
		}

		public HomogenousStackView(IEnumerable<Unit> Units, UnitConfigurationRenderer Renderer, Font Font)
		{
			_Text = new Text("", Font, FONT_SIZE) { Color = Color.Red };
			_Renderer = Renderer;
			_UnitViews = new Stack<UnitView>(Units.Select(i => new UnitView(i, Renderer, UNIT_VIEW_SCALE)));
			foreach (UnitView u in _UnitViews) u.Parent = this;
		}

		public override bool IsCollision(Vector2f Point)
		{
			return _UnitViews.First().IsCollision(Point);
		}

		public void Push(Unit Unit)
		{
			_UnitViews.Push(new UnitView(Unit, _Renderer, UNIT_VIEW_SCALE));
		}

		public Unit Pop()
		{
			return _UnitViews.Pop().Unit;
		}

		public Unit Peek()
		{
			return _UnitViews.Peek().Unit;
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			Transform.Translate(Position);
			_Text.DisplayedString = "x" + _UnitViews.Count().ToString();
			_UnitViews.First().Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			_UnitViews.First().Draw(Target, Transform);
			_Text.Position = new Vector2f(UNIT_VIEW_SCALE * .5f, UNIT_VIEW_SCALE)
				- new Vector2f(_Text.GetLocalBounds().Width, _Text.GetLocalBounds().Height) * .5f;
			_Text.Draw(Target, new RenderStates(Transform));
		}
	}
}
