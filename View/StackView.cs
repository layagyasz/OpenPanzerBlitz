using System;
using System.Collections.Generic;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class StackView : Pod
	{
		Text _LabelText;

		List<UnitView> _UnitViews = new List<UnitView>();

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

		public void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			if (Homogenous)
			{
			}
			else
			{
				Transform.Translate(-.5f * _UnitViews[0].Size * (_UnitViews.Count - 1));
				foreach (UnitView u in _UnitViews)
				{
					u.Update(MouseController, KeyController, DeltaT, Transform);
					Transform.Translate(u.Size);
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
				Transform.Translate(-.5f * _UnitViews[0].Size * (_UnitViews.Count - 1));
				foreach (UnitView u in _UnitViews)
				{
					u.Draw(Target, Transform);
					Transform.Translate(u.Size);
				}
			}
		}
	}
}
