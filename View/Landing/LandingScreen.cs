using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class LandingScreen : ScreenBase
	{
		public readonly Button LocalMatchButton = new Button("landing-button") { DisplayedString = "Local Match" };
		public readonly Button JoinRemoteMatchButton =
			new Button("landing-button") { DisplayedString = "Join Remote Match" };
		public readonly Button HostMatchButton = new Button("landing-button") { DisplayedString = "Host Match" };
		public readonly Button EditButton = new Button("landing-button") { DisplayedString = "Edit" };
		public readonly Button JoinServerButton = new Button("landing-button") { DisplayedString = "Join Server" };
		public readonly Button StartServerButton = new Button("landing-button") { DisplayedString = "Start Server" };

		ScrollCollection<ClassedGuiItem> _LandingSelect = new ScrollCollection<ClassedGuiItem>("landing-select", true);

		public LandingScreen(Vector2f WindowSize)
			: base(WindowSize, false)
		{
			_LandingSelect.Position = .5f * (WindowSize - _LandingSelect.Size);
			_LandingSelect.Add(LocalMatchButton);
			_LandingSelect.Add(JoinRemoteMatchButton);
			_LandingSelect.Add(HostMatchButton);
			_LandingSelect.Add(EditButton);
			// Commenting these out for now.  Maybe one day in the future work on PanzerBlitz online will continue!
			//_LandingSelect.Add(JoinServerButton);
			//_LandingSelect.Add(StartServerButton);

			_Items.Add(_LandingSelect);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);
			_LandingSelect.Update(MouseController, KeyController, DeltaT, Transform);

		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);
			_LandingSelect.Draw(Target, Transform);
			PaneLayer.Draw(Target, Transform);
		}
	}
}
