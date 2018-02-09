using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public interface Interaction
	{
		object Master { get; }
		OrderInvalidReason Validate();
		bool Apply();
		bool Cancel();
	}
}
