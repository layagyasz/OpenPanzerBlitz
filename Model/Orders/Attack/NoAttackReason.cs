﻿using System;
namespace PanzerBlitz
{
	public enum NoAttackReason
	{
		NONE,
		MUST_ATTACK_ALL,
		ILLEGAL_EACH,
		NOT_SPOTTED,
		OVERRUN_FORT,
		OVERRUN_TERRAIN,
		ILLEGAL
	}
}