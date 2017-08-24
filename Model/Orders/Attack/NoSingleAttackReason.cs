using System;
namespace PanzerBlitz
{
	public enum NoSingleAttackReason
	{
		NONE,
		TEAM,
		DUPLICATE,
		UNABLE,
		MUST_MOVE,
		NO_LOS,
		NO_ARMOR_ATTACK,
		OUT_OF_RANGE,
		TERRAIN,
		CONCEALED,
		NO_INDIRECT_FIRE_SPOTTER,
		PASSENGER,
		ILLEGAL
	}
}
