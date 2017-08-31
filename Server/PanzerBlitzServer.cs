using System;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class PanzerBlitzServer
	{
		ServerData _ServerData;
		Database _Database = new Database();

		public PanzerBlitzServer(string Path)
		{
			_ServerData = new ServerData(Path);
		}

		public Player LogInPlayer(string Username, string Password)
		{
			PlayerOrm p = _Database.GetPlayer(Username);
			if (p == null || !p.HasPassword(Password)) return null;
			return p.MakePlayer();
		}

		public Player RegisterPlayer(string Username, string Password)
		{
			PlayerOrm p = _Database.AddPlayer(Username, Password);
			if (p == null) return null;
			return p.MakePlayer();
		}
	}
}
