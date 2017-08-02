using System;
namespace PanzerBlitz
{
	public class IdGenerator
	{
		int _NextId = 1;

		public int GenerateId()
		{
			return _NextId++;
		}
	}
}
