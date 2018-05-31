namespace PanzerBlitz
{
	public class PanzerBlitzServer
	{
		readonly ServerData _ServerData;
		readonly Database _Database;

		public PanzerBlitzServer(string Path, Database Database)
		{
			_ServerData = new ServerData(Path);
			_Database = Database;
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
