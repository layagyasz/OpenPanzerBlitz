using System;
using System.Collections.Generic;
using System.Linq;

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
			IEnumerable<Order> orders = null;
			if (Turn.TurnInfo.TurnComponent == TurnComponent.DEPLOYMENT) orders = DoDeployment();

			if (orders == null) return;
			foreach (Order order in orders) Console.WriteLine(Match.ExecuteOrder(FileUtils.Print(order)));
			Console.WriteLine(Match.ExecuteOrder(new NextPhaseOrder(Turn.TurnInfo)));
		}

		public void Unhook() { }

		IEnumerable<Order> DoDeployment()
		{
			Root.TileEvaluations.ReEvaluate();
			Root.UnitAssignments.ReAssign();

			return Army.Deployments.SelectMany(i =>
			{
				if (i is ConvoyDeployment)
					return new ConvoyDeploymentHandler(Root).Handle((ConvoyDeployment)i);
				if (i is PositionalDeployment)
					return new PositionalDeploymentHandler(Root).Handle((PositionalDeployment)i);
				return Enumerable.Empty<Order>();
			});
		}
	}
}
