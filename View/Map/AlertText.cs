using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class AlertText : StandardItem<string>
	{
		public readonly int Duration;

		int _Time;

		public AlertText(int Duration)
			: base("screen-alert", Series.NoFocus)
		{
			this.Duration = Duration;
		}

		public void Alert(string Alert)
		{
			DisplayedString = Alert;
			_Time = 0;
			Visible = true;
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);

			if (_Time <= Duration)
			{
				_Time += DeltaT;
				Visible &= _Time <= Duration;
			}
		}
	}
}
