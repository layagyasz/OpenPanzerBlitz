using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class DeploymentSelectionOption : SelectionOption<HomogenousStackView>
	{
		public readonly UnitConfiguration UnitConfiguration;

		HomogenousStackView _StackView;

		public int Count
		{
			get
			{
				return _StackView.Count;
			}
		}

		public DeploymentSelectionOption(IEnumerable<Unit> Units, UnitConfigurationRenderer Renderer)
			: base("deployment-selection-option")
		{
			UnitConfiguration = Units.First().UnitConfiguration;

			_StackView = new HomogenousStackView(
				Units, Renderer, Class.GetAttributeWithDefault<Font>("unit-stack-font", null));
			_StackView.Position = Size / 2;
			_StackView.Parent = this;
			Value = _StackView;
		}

		public void Push(Unit Unit)
		{
			_StackView.Push(Unit);
		}

		public Unit Pop()
		{
			return _StackView.Pop();
		}

		public Unit Peek()
		{
			return _StackView.Peek();
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);

			_StackView.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);

			_StackView.Draw(Target, Transform);
		}
	}
}
