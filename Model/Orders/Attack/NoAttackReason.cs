using System;
namespace PanzerBlitz
{
	public enum NoAttackReason
	{
		NONE,
		ALREADY_ATTACKED,
		MUST_ATTACK_ALL,
		ILLEGAL_EACH,
		NOT_SPOTTED,
		OVERRUN_FORT,
		OVERRUN_TERRAIN,
		OVERRUN_EXIT,
		ILLEGAL
	}
}
