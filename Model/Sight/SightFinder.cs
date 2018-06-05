using System;

namespace PanzerBlitz
{
	public interface SightFinder
	{
		EventHandler<SightUpdatedEventArgs> OnSightUpdated { get; set; }

		void SetTrackingArmy(Army Army);
		void Hook(EventRelay Relay);

		TileSightLevel GetTileSightLevel(Tile Tile, TileSightLevel Max = TileSightLevel.HARD_SPOTTED);
		bool HasTileSightLevel(Tile Tile, TileSightLevel Level);

		bool IsSighted(Unit Unit);
	}
}
