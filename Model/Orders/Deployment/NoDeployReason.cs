using System;
namespace PanzerBlitz
{
	public enum NoDeployReason
	{
		NONE,
		STACK_LIMIT,
		ENEMY_OCCUPIED,
		UNIQUE,
		DEPLOYMENT_RULE,
		CONVOY_ORDER,
		IMPOSSIBLE
	}
}
