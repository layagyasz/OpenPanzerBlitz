namespace PanzerBlitz
{
	public class PlayerOrm
	{
		public readonly long Id;
		public readonly string Username;
		public readonly string PasswordHash;
		public readonly string PasswordSalt;

		public PlayerOrm(long Id, string Username, string PasswordHash, string PasswordSalt)
		{
			this.Id = Id;
			this.Username = Username;
			this.PasswordHash = PasswordHash;
			this.PasswordSalt = PasswordSalt;
		}

		public Player MakePlayer()
		{
			return new Player(OnlineId.Permanent(Id), Username);
		}
	}
}
