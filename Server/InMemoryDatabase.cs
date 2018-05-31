using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class InMemoryDatabase : Database
	{
		IdGenerator _IdGenerator = new IdGenerator();

		readonly Dictionary<long, PlayerOrm> _Players = new Dictionary<long, PlayerOrm>();

		public PlayerOrm AddPlayer(PlayerOrm Player)
		{
			var p = GetPlayer(Player.Username);
			if (p != null) return null;

			p = new PlayerOrm(_IdGenerator.GenerateId(), Player.Username, Player.PasswordHash, Player.PasswordSalt);
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
