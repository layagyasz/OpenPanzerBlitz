using System;
namespace PanzerBlitz
{
	public enum ProgramState
	{
		LANDING,
		EDIT,
		LOCAL_SCENARIO_SELECT,
		MATCH,
		MATCH_LOBBY,
		MATCH_END,
		SERVER,
		LOG_IN_PLAYER,
		REGISTER_PLAYER,
		ONLINE_LANDING
	}
}
