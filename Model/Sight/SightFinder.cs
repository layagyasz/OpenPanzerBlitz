using System;

namespace PanzerBlitz
{
	public interface SightFinder
	{
		EventHandler<SightUpdatedEventArgs> OnSightUpdated { get; set; }

		Army TrackingArmy { get; set; }
		void Hook(EventRelay Relay);

		TileSightLevel GetTileSightLevel(Tile Tile, TileSightLevel Max = TileSightLevel.HARD_SPOTTED);
		bool HasTileSightLevel(Tile Tile, TileSightLevel Level);
		UnitVisibility GetUnitVisibility(Unit Unit);

		bool TileConceals(Unit Unit, Tile Tile);
		bool IsSighted(Unit Unit, TileSightLevel Level);
		bool IsSighted(Unit Unit, Tile Tile);
		bool IsSighted(Unit Unit);
	}
}
