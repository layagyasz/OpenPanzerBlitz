using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	class MainClass
	{
		static Interface Interface = new Interface(VideoMode.DesktopMode, "PanzerBlitz", Styles.Default);

		public static void Main(string[] args)
		{
			ClassLibrary.Instance.ReadFile("./Theme.blk");
			GameData.Load("./BLKConfigurations");

			Interface.Screen = new Screen();

			ProgramFlowController flowController = new ProgramFlowController(Interface);

			bool edit = false;
			if (edit) flowController.EnterState(ProgramState.EDIT_STATE, null);
			else flowController.EnterState(ProgramState.LOCAL_SCENARIO_SELECT, null);
			Interface.Start(false, true);
		}
	}
}
