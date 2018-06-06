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

			SightFiringUnit(unit);

			var delta = UnitTracker.Update(this, unit);
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
				SightMovingUnit(unit, E.Path != null && E.Path.Count > 1 ? E.Path[E.Path.Count - 2] : null, E.Tile);

				var delta = UnitTracker.ComputeDelta(this, unit, E);
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
				if (OnSightUpdated != null)
				{
					var tileDeltas =
						unit.GetFieldOfSight(unit.Configuration.SightRange, E.Value)
							.Select(i => new Tuple<Tile, TileSightLevel>(i.Final, GetTileSightLevel(i.Final)))
							.ToList();
					var unitDeltas = UnitTracker.ComputeDelta(this, tileDeltas);
					unitDeltas.AddRange(UnitTracker.Remove(this, unit));
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
							UnitTracker.Remove(this, unit)));
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

		public bool IsSighted(Unit Unit, TileSightLevel Level)
		{
			return _OverrideVisibleUnits.Contains(Unit) ||
										(TileConceals(Unit, Unit.Position)
										 ? Level >= TileSightLevel.HARD_SPOTTED : Level >= TileSightLevel.SIGHTED);
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
	}
}
