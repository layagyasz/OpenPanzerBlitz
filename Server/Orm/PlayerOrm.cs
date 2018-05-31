using System;
using System.Security.Cryptography;
using System.Text;

namespace PanzerBlitz
{
	public class PlayerOrm
	{
		public readonly long Id;
		public readonly string Username;
		public readonly string PasswordHash;

		public PlayerOrm(IdGenerator IdGenerator, string Username, string Password)
		{
			Id = IdGenerator.GenerateId();
			this.Username = Username;
			PasswordHash = ComputeHash(Password);
		}

		public bool HasPassword(string Password)
		{
			return PasswordHash == ComputeHash(Password);
		}

		public Player MakePlayer()
		{
			return new Player(OnlineId.Permanent(Id), Username);
		}

		string ComputeHash(string Value)
		{
			using (SHA512 hash = SHA512.Create())
			{
				return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(Value)));
			}
		}
	}
}
