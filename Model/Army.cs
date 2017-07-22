using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Army
	{
		public readonly ArmyConfiguration Configuration;
		public readonly List<Deployment> Deployments;

		public IEnumerable<Unit> Units
		{
			get
			{
				return Deployments.SelectMany(i => i.Units);
			}
		}

		public Army(ArmyConfiguration ArmyConfiguration)
		{
			this.Configuration = ArmyConfiguration;
			this.Deployments = ArmyConfiguration.DeploymentConfigurations.Select(
				i => i.Item2.GenerateDeployment(this, i.Item1.Select(j => new Unit(this, j)))).ToList();
		}

		public bool AutomatePhase(Match Match, TurnComponent TurnComponent)
		{
			switch (TurnComponent)
			{
				case TurnComponent.ATTACK:
					return !Deployments.Any(
						i => i.Units.Any(j => j.CanAttack(AttackMethod.NORMAL_FIRE) == NoSingleAttackReason.NONE));
				case TurnComponent.CLOSE_ASSAULT:
					return !Deployments.Any(
						i => i.Units.Any(j => j.CanAttack(AttackMethod.CLOSE_ASSAULT) == NoSingleAttackReason.NONE));
				case TurnComponent.DEPLOYMENT:
					return Deployments.All(i => i.AutomateDeployment(Match));
				case TurnComponent.NON_VEHICLE_MOVEMENT:
					Deployments.ForEach(i => i.AutomateMovement(Match, false));
					return !Deployments.Any(
						i => i.Units.Any(j => j.CanMove(false, false) == NoMoveReason.NONE));
				case TurnComponent.RESET:
					foreach (Deployment d in Deployments)
						foreach (Unit u in d.Units) u.Reset();
					return true;
				case TurnComponent.VEHICLE_COMBAT_MOVEMENT:
					return !Deployments.Any(
						i => i.Units.Any(j => j.CanMove(true, true) == NoMoveReason.NONE));
				case TurnComponent.VEHICLE_MOVEMENT:
					Deployments.ForEach(i => i.AutomateMovement(Match, true));
					return !Deployments.Any(
						i => i.Units.Any(j => j.CanMove(true, false) == NoMoveReason.NONE));
			}
			return false;
		}

		public bool IsDeploymentConfigured()
		{
			return Deployments.All(i => i.IsConfigured());
		}

		public bool MustMove(bool Vehicle)
		{
			return Units.Any(i => i.MustMove() && i.CanMove(Vehicle, false) == NoMoveReason.NONE);
		}
	}
}
