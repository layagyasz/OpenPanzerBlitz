using System.Collections.Generic;

namespace PanzerBlitz
{
	public interface Formation
	{
		string Name { get; }
		IEnumerable<UnitCount> Flatten();
	}
}
