using System;
namespace PanzerBlitz
{
	public static class EnumConverter
	{
		public static NoUnloadReason ConvertToNoUnloadReason(NoDeployReason Reason)
		{
			if (Reason == NoDeployReason.NONE) return NoUnloadReason.NONE;
			if (Reason == NoDeployReason.STACK_LIMIT) return NoUnloadReason.STACK_LIMIT;
			return NoUnloadReason.ILLEGAL;
		}

		public static NoMoveReason ConvertToNoMoveReason(NoDeployReason Reason)
		{
			if (Reason == NoDeployReason.NONE) return NoMoveReason.NONE;
			if (Reason == NoDeployReason.UNIQUE) return NoMoveReason.UNIQUE;
			if (Reason == NoDeployReason.STACK_LIMIT) return NoMoveReason.STACK_LIMIT;
			if (Reason == NoDeployReason.ENEMY_OCCUPIED) return NoMoveReason.ENEMY_OCCUPIED;
			return NoMoveReason.ILLEGAL;
		}
	}
}
