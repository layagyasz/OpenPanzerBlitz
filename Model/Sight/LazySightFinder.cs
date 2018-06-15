using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LazySightFinder : SightFinder
	{
		public EventHandler<SightUpdatedEventArgs> OnSightUpdated { get; set; }

		public Army TrackingArmy
		{
			get
			{
				return _TrackingArmy;
			}
			set
			{
				UnitTracker.TrackingArmy = value;
				_TrackingArmy = value;
			}
		}

		public UnitTracker UnitTracker { get; }

		Army _TrackingArmy;
		HashSet<Unit> _OverrideVisibleUnits = new HashSet<Unit>();

		public LazySightFinder(UnitTracker UnitTracker)
		{
			this.UnitTracker = UnitTracker;
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
			Unit unit = (Unit)Sender;
			if (unit.Army == TrackingArmy) return;

			IEnumerable<Unit> otherUnits = SightFiringUnit(unit);
			var delta = UnitTracker.Update(this, unit);
			foreach (var otherUnit in otherUnits) delta.AddRange(UnitTracker.Update(this, otherUnit));
			if (OnSightUpdated != null)
			{
				OnSightUpdated(
					this,
					new SightUpdatedEventArgs(
						null,
						null,
						new List<Tuple<Tile, TileSightLevel>>(),
						delta));
			}
		}

		void HandleMove(object Sender, MovementEventArgs E)
		{
			Unit unit = (Unit)Sender;
			if (unit.Army == TrackingArmy)
			{
				HashSet<Tile> recalculateTiles = new HashSet<Tile>();
				if (unit.CanSight())
				{
					if (E.Path != null && E.Path.Count > 1)
					{
						foreach (var tile in unit.GetFieldOfSight(
							unit.Configuration.SightRange, E.Path[0]).Select(i => i.Final))
							recalculateTiles.Add(tile);
					}
					foreach (var tile in unit.GetFieldOfSight(unit.Configuration.SightRange, E.Tile)
							 .Select(i => i.Final))
						recalculateTiles.Add(tile);
				}
				var tileDeltas =
					recalculateTiles.Select(i => new Tuple<Tile, TileSightLevel>(i, GetTileSightLevel(i))).ToList();
				HandleDeltas(tileDeltas);
				var unitDeltas = UnitTracker.ComputeDelta(this, tileDeltas);
				if (OnSightUpdated != null)
				{
					OnSightUpdated(
						this,
						new SightUpdatedEventArgs(unit, E, tileDeltas, unitDeltas));
				}
			}
			else
			{
				IEnumerable<Unit> otherUnits =
					SightMovingUnit(unit, E.Path != null && E.Path.Count > 1 ? E.Path[E.Path.Count - 2] : null, E.Tile);
				var delta = UnitTracker.ComputeDelta(this, unit, E);
				foreach (var otherUnit in otherUnits) delta.AddRange(UnitTracker.Update(this, otherUnit));

				if (OnSightUpdated != null)
				{
					OnSightUpdated(
						this,
						new SightUpdatedEventArgs(
							unit,
							E,
							new List<Tuple<Tile, TileSightLevel>>(),
							delta));
				}
			}
		}

		void HandleLoad(object Sender, EventArgs E)
		{
			Unit unit = (Unit)Sender;
			if (unit.Army == TrackingArmy)
			{
				var tileDeltas =
					unit.GetFieldOfSight(unit.Configuration.SightRange, unit.Position)
						.Select(i => new Tuple<Tile, TileSightLevel>(i.Final, GetTileSightLevel(i.Final)))
						.ToList();
				HandleDeltas(tileDeltas);
				var unitDeltas = UnitTracker.ComputeDelta(this, tileDeltas);
				if (OnSightUpdated != null)
				{
					OnSightUpdated(
						this,
						new SightUpdatedEventArgs(unit, null, tileDeltas, unitDeltas));
				}
			}
			else
			{
				bool carrierOverride = _OverrideVisibleUnits.Contains(unit);
				bool passengerOverride = _OverrideVisibleUnits.Contains(unit.Passenger);

				var unitDeltas = new List<Tuple<Unit, UnitVisibility>>();
				if (carrierOverride && !passengerOverride)
				{
					_OverrideVisibleUnits.Add(unit.Passenger);
					unitDeltas.AddRange(UnitTracker.Update(this, unit.Passenger));
				}
				else if (!carrierOverride && passengerOverride)
				{
					_OverrideVisibleUnits.Add(unit);
					unitDeltas.AddRange(UnitTracker.Update(this, unit));
				}

				if (OnSightUpdated != null)
				{
					OnSightUpdated(
						this,
						new SightUpdatedEventArgs(
							unit,
							null,
							new List<Tuple<Tile, TileSightLevel>>(),
							unitDeltas));
				}
			}
		}

		void HandleUnload(object Sender, ValuedEventArgs<Unit> E)
		{
			Unit unit = (Unit)Sender;

			if (unit.Army == TrackingArmy)
			{
				var tileDeltas =
					unit.GetFieldOfSight(E.Value.Configuration.SightRange, unit.Position)
						.Select(i => new Tuple<Tile, TileSightLevel>(i.Final, GetTileSightLevel(i.Final)))
						.ToList();
				var unitDeltas = UnitTracker.ComputeDelta(this, tileDeltas);
				if (OnSightUpdated != null)
				{
					OnSightUpdated(
						this,
						new SightUpdatedEventArgs(unit, null, tileDeltas, unitDeltas));
				}
			}
			else
			{
				if (OnSightUpdated != null)
				{
					OnSightUpdated(
						this,
						new SightUpdatedEventArgs(
							unit,
							null,
							new List<Tuple<Tile, TileSightLevel>>(),
							new List<Tuple<Unit, UnitVisibility>>()));
				}
			}
		}

		void HandleRemove(object Sender, ValuedEventArgs<Tile> E)
		{
			Unit unit = (Unit)Sender;
			if (unit.Army == TrackingArmy)
			{
				var tileDeltas =
				unit.GetFieldOfSight(unit.Configuration.SightRange, E.Value)
					.Select(i => new Tuple<Tile, TileSightLevel>(i.Final, GetTileSightLevel(i.Final)))
					.ToList();
				HandleDeltas(tileDeltas);
				var unitDeltas = UnitTracker.ComputeDelta(this, tileDeltas);
				unitDeltas.AddRange(UnitTracker.Remove(this, unit));
				if (OnSightUpdated != null)
				{
					OnSightUpdated(
						this,
						new SightUpdatedEventArgs(unit, null, tileDeltas, unitDeltas));
				}
			}
			else
			{
				var unitDeltas = UnitTracker.Remove(this, unit);
				if (OnSightUpdated != null)
				{
					OnSightUpdated(
						this,
						new SightUpdatedEventArgs(
							unit,
							null,
							new List<Tuple<Tile, TileSightLevel>>(),
							unitDeltas));
				}
			}
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

		public UnitVisibility GetUnitVisibility(Unit Unit)
		{
			return UnitTracker.GetVisibility(this, Unit);
		}

		TileSightLevel GetTileSightLevel(Unit Unit, Tile Tile)
		{
			if (!Unit.CanSight()) return TileSightLevel.NONE;

			LineOfSight los = Unit.GetLineOfSight(Tile);
			if (!Unit.Configuration.IsAircraft() && los.Validate() != NoLineOfSightReason.NONE)
				return TileSightLevel.NONE;
			if (los.Range > Unit.Configuration.SightRange) return TileSightLevel.NONE;
			if (los.Range > Unit.Configuration.SpotRange || !Unit.CanSpot()) return TileSightLevel.SIGHTED;
			if (los.Range > 1) return TileSightLevel.SOFT_SPOTTED;
			return Unit.Configuration.CanReveal ? TileSightLevel.HARD_SPOTTED : TileSightLevel.SOFT_SPOTTED;
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
			if (Unit.Configuration.IsAircraft()) return true;
			return HasTileSightLevel(
				Tile, TileConceals(Unit, Tile) ? TileSightLevel.HARD_SPOTTED : TileSightLevel.SIGHTED);
		}

		public bool IsSighted(Unit Unit, TileSightLevel Level)
		{
			if (Unit.Configuration.IsAircraft()) return true;
			return _OverrideVisibleUnits.Contains(Unit) ||
										(TileConceals(Unit, Unit.Position)
										 ? Level >= TileSightLevel.HARD_SPOTTED : Level >= TileSightLevel.SIGHTED);
		}

		public bool IsSighted(Unit Unit)
		{
			return _OverrideVisibleUnits.Contains(Unit) || IsSighted(Unit, Unit.Position);
		}

		IEnumerable<Unit> SightFiringUnit(Unit Unit)
		{
			var units = new List<Unit>();
			if (Unit.Army == TrackingArmy || Unit.Position == null || IsSighted(Unit)) return units;

			if (Unit.Position != null && HasTileSightLevel(Unit.Position, TileSightLevel.SIGHTED))
			{
				_OverrideVisibleUnits.Add(Unit);
				foreach (var unit in Unit.Position.Units.Where(i => i.Covers(Unit) || Unit.Covers(i)))
				{
					_OverrideVisibleUnits.Add(Unit);
					units.Add(unit);
				}
			}
			return units;
		}

		IEnumerable<Unit> SightMovingUnit(Unit Unit, Tile MovedFrom, Tile MovedTo)
		{
			var units = new List<Unit>();
			if (Unit.Army == TrackingArmy || !MovedTo.Rules.Concealing) return units;
			if (IsSighted(Unit, MovedFrom))
			{
				_OverrideVisibleUnits.Add(Unit);
				foreach (var unit in Unit.Position.Units.Where(i => i.Covers(Unit) || Unit.Covers(i)))
				{
					_OverrideVisibleUnits.Add(Unit);
					units.Add(unit);
				}
			}
			else _OverrideVisibleUnits.Remove(Unit);
			return units;
		}

		void HandleDeltas(IEnumerable<Tuple<Tile, TileSightLevel>> Deltas)
		{
			foreach (var delta in Deltas.Where(i => i.Item2 == TileSightLevel.NONE))
			{
				foreach (var unit in delta.Item1.Units.Where(i => i.Army != TrackingArmy))
					_OverrideVisibleUnits.Remove(unit);
			}
		}
	}
}
