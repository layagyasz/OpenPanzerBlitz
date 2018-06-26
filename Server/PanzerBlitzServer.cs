using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PanzerBlitz
{
	public class PanzerBlitzServer
	{
		readonly ServerData _ServerData;
		readonly RandomNumberGenerator _Rng;
		readonly Database _Database;

		public PanzerBlitzServer(string Path, RandomNumberGenerator Rng, Database Database)
		{
			_ServerData = new ServerData(Path);
			_Rng = Rng;
			_Database = Database;
		}

		public Player LogInPlayer(string Username, string Password)
		{
			var p = _Database.GetPlayer(Username);
			if (p == null || p.PasswordHash != ComputeHash(Password, p.PasswordSalt)) return null;
			return p.MakePlayer();
		}

		public Player RegisterPlayer(string Username, string Password)
		{
			var salt = MakeSalt();
			var p = _Database.AddPlayer(new PlayerOrm(0, Username, ComputeHash(Password, salt), salt));
			if (p == null) return null;
			return p.MakePlayer();
		}

		public List<UnitConfigurationPack> GetUnitConfigurationPacks()
		{
			return _ServerData.UnitConfigurationPacks;
		}

		string MakeSalt()
		{
			byte[] bytes = new byte[6];
			_Rng.GetBytes(bytes);
			return Convert.ToBase64String(bytes);
		}

		string ComputeHash(string Value, string Salt)
		{
			using (SHA512 hash = SHA512.Create())
			{
				return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(Value + Salt)));
			}
		}
	}
}
