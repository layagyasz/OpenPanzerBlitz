using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LazySightFinder : SightFinder
	{
		public EventHandler<SightUpdatedEventArgs> OnSightUpdated { get; set; }

		public Army TrackingArmy { get; private set; }

		HashSet<Unit> _OverrideVisibleUnits = new HashSet<Unit>();

		public void SetTrackingArmy(Army Army)
		{
			TrackingArmy = Army;
		}

		public void Hook(EventRelay Relay)
		{
			Relay.OnUnitMove += HandleMove;
			Relay.OnUnitLoad += HandleLoad;
			Relay.OnUnitUnload += HandleUnload;
			Relay.OnUnitRemove += HandleRemove;
			Relay.OnUnitFire += HandleFire;
		}

		void HandleFire(object Sender, EventArgs E)
		{
			SightFiringUnit((Unit)Sender);
		}

		void HandleMove(object Sender, MovementEventArgs E)
		{
			Unit unit = (Unit)Sender;
			if (unit.Army == TrackingArmy) UpdateFriendlyMove(unit, E);
			else SightMovingUnit(unit, E.Path != null && E.Path.Count > 1 ? E.Path[E.Path.Count - 2] : null, E.Tile);
		}

		void HandleLoad(object Sender, EventArgs E)
		{
			Unit unit = (Unit)Sender;
			if (unit.Army == TrackingArmy) UpdateFriendlyUnmoved(unit, unit.Position);
		}

		void HandleUnload(object Sender, ValuedEventArgs<Unit> E)
		{
			if (E.Value.Army == TrackingArmy) UpdateFriendlyUnmoved(E.Value, E.Value.Position);
		}

		void HandleRemove(object Sender, ValuedEventArgs<Tile> E)
		{
			Unit unit = (Unit)Sender;
			if (unit.Army == TrackingArmy) UpdateFriendlyUnmoved(unit, E.Value);
		}

		public TileSightLevel GetTileSightLevel(Tile Tile, TileSightLevel Max = TileSightLevel.HARD_SPOTTED)
		{
			if (Tile == null) return TileSightLevel.NONE;

			var max = TileSightLevel.NONE;
			foreach (var unit in TrackingArmy.Units)
			{
				var level = GetTileSightLevel(unit, Tile);
				if (level > max) max = level;
				if (level >= Max) return max;
			}
			return max;
		}

		public bool HasTileSightLevel(Tile Tile, TileSightLevel Level)
		{
			return GetTileSightLevel(Tile, Level) >= Level;
		}

		TileSightLevel GetTileSightLevel(Unit Unit, Tile Tile)
		{
			if (!Unit.CanSight()) return TileSightLevel.NONE;

			LineOfSight los = Unit.GetLineOfSight(Tile);
			if (los.Validate() != NoLineOfSightReason.NONE) return TileSightLevel.NONE;
			if (los.Range > Math.Max((byte)20, Unit.Configuration.GetAdjustedRange(false))) return TileSightLevel.NONE;
			if (los.Range > Unit.Configuration.SpotRange || !Unit.CanSpot()) return TileSightLevel.SIGHTED;
			if (los.Range > 1) return TileSightLevel.SOFT_SPOTTED;
			return TileSightLevel.HARD_SPOTTED;
		}

		public bool TileConceals(Unit Unit, Tile Tile)
		{
			if (Tile == null) return false;
			if (Unit.Configuration.IsAircraft()) return false;
			if (Tile.Rules.Concealing) return true;
			if (Tile.Rules.LowProfileConcealing)
			{
				return Unit.Configuration.HasLowProfile
					  || Unit.Position.Units.Any(
						  i => i.Configuration.UnitClass == UnitClass.FORT
							   && i.Army == Unit.Army
							   && i.Configuration.HasLowProfile);
			}
			return false;
		}

		public bool IsSighted(Unit Unit, Tile Tile)
		{
			return HasTileSightLevel(
				Tile, TileConceals(Unit, Tile) ? TileSightLevel.HARD_SPOTTED : TileSightLevel.SIGHTED);
		}

		public bool IsSighted(Unit Unit)
		{
			return _OverrideVisibleUnits.Contains(Unit) || IsSighted(Unit, Unit.Position);
		}

		public void SightFiringUnit(Unit Unit)
		{
			if (Unit.Army == TrackingArmy || Unit.Position == null || IsSighted(Unit)) return;

			if (Unit.Position != null && HasTileSightLevel(Unit.Position, TileSightLevel.SIGHTED))
				_OverrideVisibleUnits.Add(Unit);
			else _OverrideVisibleUnits.Remove(Unit);
		}

		public void SightMovingUnit(Unit Unit, Tile MovedFrom, Tile MovedTo)
		{
			if (Unit.Army == TrackingArmy || !MovedTo.Rules.Concealing) return;
			if (IsSighted(Unit, MovedFrom)) _OverrideVisibleUnits.Add(Unit);
			else _OverrideVisibleUnits.Remove(Unit);
		}

		void UpdateFriendlyUnmoved(Unit Unit, Tile Tile)
		{
			if (OnSightUpdated != null)
				OnSightUpdated(
					this,
					new SightUpdatedEventArgs(
						Unit.GetFieldOfSight(
							Math.Max((byte)20, Unit.Configuration.GetAdjustedRange(false)), Tile)
						.Select(i => new Tuple<Tile, TileSightLevel>(i.Final, GetTileSightLevel(i.Final)))
						.ToList()));
		}

		void UpdateFriendlyMove(Unit Unit, MovementEventArgs E)
		{
			if (!Unit.CanSight()) return;

			HashSet<Tile> recalculateTiles = new HashSet<Tile>();
			if (E.Path != null && E.Path.Count > 1)
			{
				foreach (var tile in Unit.GetFieldOfSight(
					Math.Max((byte)20, Unit.Configuration.GetAdjustedRange(false)), E.Path[0]).Select(i => i.Final))
					recalculateTiles.Add(tile);
			}
			foreach (var tile in Unit.GetFieldOfSight(
				Math.Max((byte)20, Unit.Configuration.GetAdjustedRange(false)), E.Tile).Select(i => i.Final))
				recalculateTiles.Add(tile);
			if (OnSightUpdated != null)
				OnSightUpdated(
					this,
					new SightUpdatedEventArgs(
						recalculateTiles.Select(i => new Tuple<Tile, TileSightLevel>(i, GetTileSightLevel(i)))
						.ToList()));
		}
	}
}
