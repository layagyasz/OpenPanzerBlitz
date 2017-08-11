﻿using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class LandingScreen : ScreenBase
	{
		public readonly PaneLayer PaneLayer = new PaneLayer();
		public readonly Button LocalMatchButton = new Button("landing-button") { DisplayedString = "Local Match" };
		public readonly Button JoinRemoteMatchButton =
			new Button("landing-button") { DisplayedString = "Join Remote Match" };
		public readonly Button HostMatchButton = new Button("landing-button") { DisplayedString = "Host Match" };
		public readonly Button EditButton = new Button("landing-button") { DisplayedString = "Edit" };

		ScrollCollection<object> _LandingSelect = new ScrollCollection<object>("landing-select", true);

		public LandingScreen(Vector2f WindowSize)
			: base(WindowSize)
		{
			_LandingSelect.Position = .5f * (WindowSize - _LandingSelect.Size);
			_LandingSelect.Add(LocalMatchButton);
			_LandingSelect.Add(JoinRemoteMatchButton);
			_LandingSelect.Add(HostMatchButton);
			_LandingSelect.Add(EditButton);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			_LandingSelect.Update(MouseController, KeyController, DeltaT, Transform);
			PaneLayer.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);
			_LandingSelect.Draw(Target, Transform);
			PaneLayer.Draw(Target, Transform);
		}
	}
}