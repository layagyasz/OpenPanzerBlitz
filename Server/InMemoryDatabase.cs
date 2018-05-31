using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class InMemoryDatabase : Database
	{
		IdGenerator _IdGenerator = new IdGenerator();

		readonly Dictionary<long, PlayerOrm> _Players = new Dictionary<long, PlayerOrm>();

		public PlayerOrm AddPlayer(string Username, string Password)
		{
			var p = GetPlayer(Username);
			if (p == null) return null;
			p = new PlayerOrm(_IdGenerator, Username, Password);
			lock (_Players)
			{
				_Players.Add(p.Id, p);
			}
			return p;
		}

		public PlayerOrm GetPlayer(string Username)
		{
			return _Players.FirstOrDefault(i => i.Value.Username == Username).Value;
		}
	}
}
