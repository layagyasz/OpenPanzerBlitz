using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public interface Interaction
	{
		Unit Master { get; }
		OrderInvalidReason Validate();
		bool Apply(Unit Unit);
		bool Cancel();
	}
}
