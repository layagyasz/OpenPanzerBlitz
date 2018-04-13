namespace PanzerBlitz
{
	public class AIMatchPlayerController : MatchPlayerController
	{
		public readonly MatchAdapter Match;
		public readonly Army Army;
		public readonly AIRoot Root;

		public AIMatchPlayerController(MatchAdapter Match, Army Army)
		{
			this.Match = Match;
			this.Army = Army;
			Root = new AIRoot(Match, Army);
		}

		public void DoTurn(Turn Turn)
		{
			if (Turn.TurnInfo.TurnComponent == TurnComponent.DEPLOYMENT) DoDeployment();

			Match.ExecuteOrder(new NextPhaseOrder(Army));
		}

		public void Unhook() { }

		void DoDeployment()
		{
			Root.TileEvaluations.ReEvaluate();
			Root.ReAssign();

			foreach (var deployment in Army.Deployments)
			{
				if (deployment is ConvoyDeployment)
				{
					foreach (Order order in new ConvoyDeploymentHandler(Root).Handle((ConvoyDeployment)deployment))
						Match.ExecuteOrder(order);
				}
			}
		}
	}
}
