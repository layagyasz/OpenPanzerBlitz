using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class Deployment : GameObject
	{
		int _Id;

		public readonly Army Army;
		public readonly List<Unit> Units;
		public abstract DeploymentConfiguration Configuration { get; }

		public int Id
		{
			get
			{
				return _Id;
			}
		}

		public Deployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
		{
			this.Army = Army;
			this.Units = Units.ToList();
			_Id = IdGenerator.GenerateId();
		}

		public virtual NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			if (Tile == null) return NoDeployReason.NONE;
			return Unit.CanEnter(Tile, true);
		}

		public abstract bool AutomateDeployment(Match Match);
		public abstract void AutomateMovement(Match Match, bool Vehicle);
		public abstract bool IsConfigured();
	}
}
