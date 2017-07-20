using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class TileEntryDeployment : ConvoyDeployment
	{
		public TileEntryDeployment(IEnumerable<Unit> Units)
			: base(Units)
		{
		}

		public override bool AutomateDeployment(Match Match)
		{
			return false;
		}

		public override bool IsConfigured()
		{
			return Validate(_EntryTile) == NoDeployReason.NONE && Validate(_ConvoyOrder) == NoDeployReason.NONE;
		}

		public override string GetDisplayString()
		{
			return "Tile Entry Deployment";
		}
	}
}
