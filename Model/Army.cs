using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Army
	{
		public EventHandler<StartTurnComponentEventArgs> OnStartPhase;
		public EventHandler<StartTurnComponentEventArgs> OnEndPhase;

		public readonly ArmyConfiguration ArmyConfiguration;
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
			this.ArmyConfiguration = ArmyConfiguration;
			this.Deployments = ArmyConfiguration.DeploymentConfigurations.Select(
				i => new Deployment(this, i)).ToList();
		}

		public void StartPhase(TurnComponent TurnComponent)
		{
			if (OnStartPhase != null) OnStartPhase(this, new StartTurnComponentEventArgs(TurnComponent));
		}

		public void EndPhase(TurnComponent TurnComponent)
		{
			if (OnEndPhase != null) OnEndPhase(this, new StartTurnComponentEventArgs(TurnComponent));
		}

		public void Deploy()
		{
			StartPhase(TurnComponent.DEPLOYMENT);
		}

		public bool IsDeployed()
		{
			return Units.All(i => i.Deployed);
		}
	}
}
