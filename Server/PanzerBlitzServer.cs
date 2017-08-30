using System;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class PanzerBlitzServer
	{
		ServerData _ServerData;

		public PanzerBlitzServer(string Path)
		{
			_ServerData = new ServerData(Path);
		}
	}
}
