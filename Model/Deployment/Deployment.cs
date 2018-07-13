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

		protected Deployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
		{
			this.Army = Army;
			this.Units = Units.ToList();
			_Id = IdGenerator.GenerateId();
		}

		public virtual OrderInvalidReason Validate(Unit Unit, Tile Tile)
		{
			if (Tile == null) return OrderInvalidReason.NONE;
			return Unit.CanEnter(Tile, true);
		}

		public virtual bool AutomateDeployment()
		{
			return false;
		}
		public virtual bool UnitMustMove(Unit Unit)
		{
			return false;
		}
		public virtual void EnterUnits(Turn Turn, bool Vehicle) { }

		public virtual void ReassessMatch() { }

		public virtual void AutomateMovement(bool Vehicle) { }

		public abstract bool IsConfigured();
	}
}
