﻿namespace PanzerBlitz
{
	public enum OrderInvalidReason
	{
		NONE,
		ILLEGAL,
		ORDER_TURN_COMPONENT,

		TILE_ENEMY_OCCUPIED,
		UNIT_NO_ACTION,
		UNIT_UNIQUE,
		UNIT_STACK_LIMIT,
		UNIT_NO_MOVE,
		UNIT_NO_CARRY,
		UNIT_NO_DISMOUNT,
		UNIT_MUST_MOVE,
		UNIT_CARRYING,
		UNIT_NO_PASSENGER,
		UNIT_NO_ATTACK,
		UNIT_DUPLICATE,
		UNIT_NO_REMOUNT,
		UNIT_CLOSE_ASSAULT_SUPPORT,
		UNIT_NO_CARRY_IN_WATER,
		UNIT_NO_ENGINEER,

		TARGET_EMPTY,
		TARGET_ALREADY_ATTACKED,
		TARGET_CARRIED,
		TARGET_TEAM,
		TARGET_OUT_OF_RANGE,
		TARGET_CONCEALED,
		TARGET_ARMORED,
		TARGET_IMMUNE,
		TARGET_COVERED,

		OVERRUN_TERRAIN,
		OVERRUN_FORT,
		OVERRUN_EXIT,

		MOVEMENT_TERRAIN,

		DEPLOYMENT_RULE,
		DEPLOYMENT_CONVOY_ORDER,

		MUST_ATTACK_ALL,
		MUST_ATTACK_EACH,
		ILLEGAL_ATTACK_EACH,

		ATTACK_NO_LOS,
		ATTACK_NO_SPOTTER
	}
}
