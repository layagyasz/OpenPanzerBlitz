using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class ServerStateController : ProgramStateController
	{
		ServerContext _ServerContext;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_ServerContext = (ServerContext)ProgramStateContext;
			return new ServerScreen(ProgramContext.ScreenResolution);
		}
	}
}
