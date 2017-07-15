using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class StackView : Pod
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

		public void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			Transform.Translate(-.5f * STEP * _UnitViews[0].Size * (_UnitViews.Count - 1));
			foreach (UnitView u in _UnitViews)
			{
				u.Update(MouseController, KeyController, DeltaT, Transform);
				Transform.Translate(STEP * u.Size);
			}

		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			Transform.Translate(-.5f * STEP * _UnitViews[0].Size * (_UnitViews.Count - 1));
			foreach (UnitView u in _UnitViews)
			{
				u.Draw(Target, Transform);
				Transform.Translate(STEP * u.Size);
			}

		}
	}
}
