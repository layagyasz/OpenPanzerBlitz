namespace PanzerBlitz
{
	public class PanzerBlitzServer
	{
		readonly ServerData _ServerData;
		readonly Database _Database = new Database();

		public PanzerBlitzServer(string Path)
		{
			_ServerData = new ServerData(Path);
		}

		public Player LogInPlayer(string Username, string Password)
		{
			var p = _Database.GetPlayer(Username);
			if (p == null || !p.HasPassword(Password)) return null;
			return p.MakePlayer();
		}

		public Player RegisterPlayer(string Username, string Password)
		{
			var p = _Database.AddPlayer(Username, Password);
			if (p == null) return null;
			return p.MakePlayer();
		}
	}
}
