using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class LogInPlayerStateController : ProgramStateController
	{
		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			return new LogInPlayerScreen(ProgramContext.ScreenResolution);
		}
	}
}
