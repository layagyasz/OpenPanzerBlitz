using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Army : GameObject
	{
		public EventHandler<NewUnitEventArgs> OnUnitAdded;

		public readonly ArmyConfiguration Configuration;
		public readonly List<Deployment> Deployments;

		int _Id;
		IdGenerator _IdGenerator;

		public int Id
		{
			get
			{
				return _Id;
			}
		}
		public IEnumerable<Unit> Units
		{
			get
			{
				return Deployments.SelectMany(i => i.Units);
			}
		}

		public Army(ArmyConfiguration ArmyConfiguration, IdGenerator IdGenerator)
		{
			_Id = IdGenerator.GenerateId();
			Configuration = ArmyConfiguration;
			Deployments = ArmyConfiguration.DeploymentConfigurations.Select(
				i => i.Item2.GenerateDeployment(
					this, i.Item1.Select(j => new Unit(this, j, IdGenerator)), IdGenerator)).ToList();
			foreach (Unit u in Units) u.OnDestroy += UnitDestroyed;
			_IdGenerator = IdGenerator;
		}

		public bool AutomatePhase(Match Match, TurnComponent TurnComponent, Random Random)
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
				case TurnComponent.MINEFIELD_ATTACK:
					DoMinefieldAttacks(Random);
					return true;
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

		public ObjectiveSuccessLevel GetObjectiveSuccessLevel(Match Match)
		{
			return Configuration.VictoryCondition.GetMatchResult(this, Match);
		}

		public IEnumerable<GameObject> GetGameObjects()
		{
			return Enumerable.Repeat(this, 1).Concat<GameObject>(Deployments).Concat(Units);
		}

		void DoMinefieldAttacks(Random Random)
		{
			foreach (Unit u in Units)
			{
				if (u.Position == null) continue;

				Unit mine = u.Position.Units.FirstOrDefault(i => i.Configuration.UnitClass == UnitClass.MINEFIELD);
				if (mine != null && mine.Position != null)
				{
					foreach (Unit d in mine.Position.Units.Where(i => i != mine))
					{
						AttackOrder order = new AttackOrder(mine.Army, u.Position, AttackMethod.MINEFIELD);
						order.AddAttacker(new MinefieldSingleAttackOrder(mine, d));
						order.Execute(Random);
					}
				}
			}
		}

		public bool IsDeploymentConfigured()
		{
			return Deployments.All(i => i.IsConfigured());
		}

		public bool MustMove(bool Vehicle)
		{
			return Units.Any(i => i.MustMove() && i.CanMove(Vehicle, false) == NoMoveReason.NONE);
		}

		void UnitDestroyed(object Sender, EventArgs E)
		{
			Unit unit = (Unit)Sender;
			if (unit.Configuration.IsArmored && unit.Configuration.IsVehicle)
			{
				Unit wreckage = new Unit(this, GameData.Wreckage, _IdGenerator);
				if (OnUnitAdded != null) OnUnitAdded(this, new NewUnitEventArgs(wreckage));
				wreckage.Place(unit.Position);
			}
		}
	}
}
