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

		Text _LabelText;

		List<UnitView> _UnitViews = new List<UnitView>();

		public int Count
		{
			get
			{
				return _UnitViews.Count;
			}
		}

		public bool Homogenous
		{
			get
			{
				return _LabelText != null;
			}
		}

		public StackView()
		{
		}

		public StackView(Text LabelText)
		{
			_LabelText = new Text(LabelText);
		}

		public void Add(UnitView UnitView)
		{
			_UnitViews.Add(UnitView);
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
			if (Homogenous)
			{
			}
			else
			{
				Transform.Translate(-.5f * STEP * _UnitViews[0].Size * (_UnitViews.Count - 1));
				foreach (UnitView u in _UnitViews)
				{
					u.Update(MouseController, KeyController, DeltaT, Transform);
					Transform.Translate(STEP * u.Size);
				}
			}
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			if (Homogenous)
			{
			}
			else
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
}
