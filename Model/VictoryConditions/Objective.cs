using System;
namespace PanzerBlitz
{
	public interface Objective
	{
		int CalculateScore(Army ForArmy, Match Match);
		int GetScore();
	}
}
