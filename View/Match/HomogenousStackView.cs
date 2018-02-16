using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class HomogenousStackView : Interactive
	{
		readonly Button _Text;
		readonly UnitConfigurationRenderer _Renderer;
		readonly Stack<UnitView> _UnitViews;
		readonly int _UnitScale;

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

		public HomogenousStackView(
			IEnumerable<Unit> Units, UnitConfigurationRenderer Renderer, int UnitScale, string OverlayClassName)
		{
			_Text = new Button(OverlayClassName);
			_Text.Position = -.5f * new Vector2f(UnitScale, _Text.Size.Y);
			_Text.Parent = this;

			_UnitScale = UnitScale;
			_Renderer = Renderer;
			_UnitViews = new Stack<UnitView>(Units.Select(i => new UnitView(i, Renderer, _UnitScale, false)));
			foreach (UnitView u in _UnitViews) u.Parent = this;
		}

		public override bool IsCollision(Vector2f Point)
		{
			return _UnitViews.First().IsCollision(Point);
		}

		public void Push(Unit Unit)
		{
			_UnitViews.Push(new UnitView(Unit, _Renderer, _UnitScale, false));
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
			_Text.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(Position);
			_UnitViews.First().Draw(Target, Transform);
			_Text.Draw(Target, Transform);
		}
	}
}
