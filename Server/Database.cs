using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Database
	{
		IdGenerator _IdGenerator = new IdGenerator();

		readonly Dictionary<int, PlayerOrm> _Players = new Dictionary<int, PlayerOrm>();

		public PlayerOrm AddPlayer(string Username, string Password)
		{
			var p = GetPlayer(Username);
			if (p == null) return null;
			p = new PlayerOrm(_IdGenerator, Username, Password);
			_Players.Add(p.Id, p);
			return p;
		}

		public PlayerOrm GetPlayer(string Username)
		{
			return _Players.FirstOrDefault(i => i.Value.Username == Username).Value;
		}
	}
}
