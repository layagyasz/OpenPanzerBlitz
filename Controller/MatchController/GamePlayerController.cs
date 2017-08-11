﻿using System;
namespace PanzerBlitz
{
	public interface GamePlayerController
	{
		void DoTurn(TurnInfo TurnInfo);
		void ExecuteOrder(Order Order);
	}
}