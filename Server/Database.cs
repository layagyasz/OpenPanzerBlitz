namespace PanzerBlitz
{
	public interface Database
	{
		PlayerOrm AddPlayer(PlayerOrm Player);
		PlayerOrm GetPlayer(string Username);
	}
}
