using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class FogOfWarUnitTracker : UnitTracker
	{
		public Army TrackingArmy { get; set; }

		readonly Dictionary<Unit, UnitVisibility> _UnitVisibilities = new Dictionary<Unit, UnitVisibility>();

		public UnitVisibility GetVisibility(SightFinder SightFinder, Unit Unit)
		{
			if (Unit.Army == TrackingArmy) return new UnitVisibility(Unit.Position != null, Unit.Position);

			UnitVisibility visibility;
			_UnitVisibilities.TryGetValue(Unit, out visibility);
			return visibility;
		}

		public List<Tuple<Unit, UnitVisibility>> Update(SightFinder SightFinder, Unit Unit)
		{
			if (Unit.Army == TrackingArmy)
			{
				return new List<Tuple<Unit, UnitVisibility>>
				{
					new Tuple<Unit, UnitVisibility>(Unit, new UnitVisibility(Unit.Position != null, Unit.Position))
				};
			}
			return new List<Tuple<Unit, UnitVisibility>>
			{
				new Tuple<Unit, UnitVisibility>(Unit, MergeVisibility(Unit, SightFinder.IsSighted(Unit)))
			};
		}

		public List<Tuple<Unit, UnitVisibility>> Remove(SightFinder SightFinder, Unit Unit)
		{
			if (Unit.Army == TrackingArmy)
			{
				return new List<Tuple<Unit, UnitVisibility>>
				{
					new Tuple<Unit, UnitVisibility>(Unit, new UnitVisibility(false, null))
				};
			}
			return new List<Tuple<Unit, UnitVisibility>>
			{
				new Tuple<Unit, UnitVisibility>(Unit, MergeVisibility(Unit, false, true))
			};
		}

		public List<Tuple<Unit, UnitVisibility>> ComputeDelta(
			SightFinder SightFinder, Unit Unit, MovementEventArgs Movement)
		{
			if (Unit.Army == TrackingArmy)
			{
				return new List<Tuple<Unit, UnitVisibility>>
				{
					new Tuple<Unit, UnitVisibility>(Unit, new UnitVisibility(true, Unit.Position))
				};
			}
			var visible = SightFinder.IsSighted(Unit);
			Tile lastSeen = null;
			if (Movement != null && Movement.Path != null)
			{
				for (int i = Movement.Path.Count - 2; i >= 0; --i)
				{
					if (SightFinder.IsSighted(Unit, Movement.Path[i]))
					{
						lastSeen = Movement.Path[i + 1];
						break;
					}
				}
			}
			return new List<Tuple<Unit, UnitVisibility>>
			{
				new Tuple<Unit, UnitVisibility>(Unit, MergeVisibility(Unit, visible, OverrideLastSeen: lastSeen))
			};
		}

		public List<Tuple<Unit, UnitVisibility>> ComputeDelta(
			SightFinder SightFinder, List<Tuple<Tile, TileSightLevel>> TileDeltas)
		{
			var deltas = new List<Tuple<Unit, UnitVisibility>>();
			foreach (var tileDelta in TileDeltas)
			{
				foreach (var unit in tileDelta.Item1.Units.Where(i => i.Army != TrackingArmy))
				{
					deltas.Add(
						new Tuple<Unit, UnitVisibility>(
							unit, MergeVisibility(unit, SightFinder.IsSighted(unit, tileDelta.Item2))));
				}
				if (tileDelta.Item2 == TileSightLevel.HARD_SPOTTED
					|| (tileDelta.Item2 != TileSightLevel.NONE
						&& !tileDelta.Item1.Rules.Concealing
						&& !tileDelta.Item1.Rules.LowProfileConcealing))
				{
					foreach (var unit in _UnitVisibilities.Where(
						i => i.Value.LastSeen == tileDelta.Item1 && i.Key.Position != i.Value.LastSeen).ToList())
					{
						if (tileDelta.Item2 == TileSightLevel.HARD_SPOTTED
							|| !SightFinder.TileConceals(unit.Key, tileDelta.Item1))
						{
							deltas.Add(
								new Tuple<Unit, UnitVisibility>(unit.Key, MergeVisibility(unit.Key, false, true)));
						}
					}
				}
			}
			return deltas;
		}

		UnitVisibility MergeVisibility(Unit Unit, bool Visible, bool Remove = false, Tile OverrideLastSeen = null)
		{
			UnitVisibility visibility;
			_UnitVisibilities.TryGetValue(Unit, out visibility);

			if (Visible) visibility = new UnitVisibility(Visible, Unit.Position);
			else visibility = new UnitVisibility(Visible, Remove ? null : (OverrideLastSeen ?? visibility.LastSeen));

			_UnitVisibilities[Unit] = visibility;
			return visibility;
		}
	}
}
