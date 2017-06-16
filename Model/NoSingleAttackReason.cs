using System;
namespace PanzerBlitz
{
	public enum NoSingleAttackReason
	{
		NONE,
		TEAM,
		NO_DIRECT_FIRE,
		NO_INDIRECT_FIRE,
		NO_AA_FIRE,
		NO_LOS,
		NO_ARMOR_ATTACK,
		OUT_OF_RANGE,
		NO_INDIRECT_FIRE_SPOTTER
	}
}
