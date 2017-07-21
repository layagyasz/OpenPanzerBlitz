using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class StackView : Interactive
	{
		static readonly float STEP = .25f;

		List<UnitView> _UnitViews = new List<UnitView>();

		public int Count
		{
			get
			{
				return _UnitViews.Count;
			}
		}
		public override Vector2f Size
		{
			get
			{
				return new Vector2f(0, 0);
			}
		}

		public override bool IsCollision(Vector2f Point)
		{
			return false;
		}

		public void Sort()
		{
			_UnitViews.Sort(new StackViewUnitComparator());
		}

		public void Add(UnitView UnitView)
		{
			_UnitViews.Add(UnitView);
			Sort();
		}

		public bool Contains(Unit Unit)
		{
			return _UnitViews.Any(i => i.Unit == Unit);
		}

		public void Remove(Unit Unit)
		{
			_UnitViews.RemoveAll(i => i.Unit == Unit);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);

			Transform.Translate(-.5f * STEP * _UnitViews[0].Size * (_UnitViews.Count - 1) + Position);
			foreach (UnitView u in _UnitViews)
			{
				u.Update(MouseController, KeyController, DeltaT, Transform);
				Transform.Translate(STEP * u.Size);
			}

		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(-.5f * STEP * _UnitViews[0].Size * (_UnitViews.Count - 1) + Position);
			foreach (UnitView u in _UnitViews)
			{
				u.Draw(Target, Transform);
				Transform.Translate(STEP * u.Size);
			}

		}
	}
}
