using System;
namespace PanzerBlitz
{
	public enum NoSingleAttackReason
	{
		NONE,
		TEAM,
		DUPLICATE,
		UNABLE,
		NO_LOS,
		NO_ARMOR_ATTACK,
		OUT_OF_RANGE,
		NO_INDIRECT_FIRE_SPOTTER
	}
}
