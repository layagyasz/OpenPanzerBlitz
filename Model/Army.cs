using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class Army : GameObject
	{
		public EventHandler<NewUnitEventArgs> OnUnitAdded;

		public readonly ArmyConfiguration Configuration;
		public readonly List<Deployment> Deployments;

		int _Id;
		IdGenerator _IdGenerator;

		HashSet<Tile> _AttackedTiles = new HashSet<Tile>();
		HashSet<Unit> _OverrideVisibleUnits = new HashSet<Unit>();

		public int Id
		{
			get
			{
				return _Id;
			}
		}
		public IEnumerable<Unit> Units
		{
			get
			{
				return Deployments.SelectMany(i => i.Units);
			}
		}

		public Army(ArmyConfiguration ArmyConfiguration, IdGenerator IdGenerator)
		{
			_Id = IdGenerator.GenerateId();
			Configuration = ArmyConfiguration;
			Deployments = ArmyConfiguration.DeploymentConfigurations.Select(
				i => i.Item2.GenerateDeployment(
					this, i.Item1.Select(j => new Unit(this, j, IdGenerator)), IdGenerator)).ToList();
			foreach (Unit u in Units) u.OnDestroy += UnitDestroyed;
			_IdGenerator = IdGenerator;
		}

		public ObjectiveSuccessLevel GetObjectiveSuccessLevel(Match Match)
		{
			return Configuration.VictoryCondition.GetMatchResult(this, Match);
		}

		public IEnumerable<GameObject> GetGameObjects()
		{
			return Enumerable.Repeat(this, 1).Concat<GameObject>(Deployments).Concat(Units);
		}

		public bool IsDeploymentConfigured()
		{
			return Deployments.All(i => i.IsConfigured());
		}

		public void Reset(bool CompleteReset)
		{
			if (CompleteReset)
			{
				foreach (Unit u in Units) u.Reset();
			}
			_AttackedTiles.Clear();
		}

		public void AttackTile(Tile Tile)
		{
			_AttackedTiles.Add(Tile);
		}

		public bool HasAttackedTile(Tile Tile)
		{
			return _AttackedTiles.Contains(Tile);
		}

		public bool MustMove(bool Vehicle)
		{
			return Units.Any(i => i.MustMove() && i.CanMove(Vehicle, false) == NoMoveReason.NONE);
		}

		public bool CanSeeUnit(Unit Unit)
		{
			if (Unit.Position == null) return false;
			return !Unit.Position.Configuration.Concealing
						|| CanSpotTile(Unit.Position)
						|| _OverrideVisibleUnits.Contains(Unit);
		}

		public bool CanSeeTile(Tile Tile, bool OverrideConcealment = false)
		{
			if (Tile == null) return false;
			if (Tile.Configuration.Concealing && !OverrideConcealment)
				return CanSpotTile(Tile);
			foreach (Unit u in Units)
			{
				LineOfSight s = u.GetLineOfSight(Tile);
				if (s != null && s.Validate() == NoLineOfSightReason.NONE) return true;
			}
			return false;
		}

		public bool CanIndirectFireAtTile(Tile Tile)
		{
			if (!CanSpotTile(Tile)) return false;
			foreach (Unit u in Units.Where(i => i.Configuration.CanSpotIndirectFire))
			{
				LineOfSight s = u.GetLineOfSight(Tile);
				if (s != null && s.Validate() == NoLineOfSightReason.NONE) return true;
			}
			return false;
		}

		public bool CanSpotTile(Tile Tile)
		{
			if (Tile == null) return false;
			if (!Tile.Configuration.Concealing) return true;
			return Units.Any(i => i.Position != null && (i.Position == Tile || i.Position.Neighbors().Contains(Tile)));
		}

		public void SetUnitVisibility(Unit Unit, bool Visible)
		{
			if (Unit.Army == this || Unit.Position == null || !Unit.Position.Configuration.Concealing) return;

			if (Unit.Position != null && CanSeeTile(Unit.Position, true))
			{
				if (Visible) _OverrideVisibleUnits.Add(Unit);
				else _OverrideVisibleUnits.Remove(Unit);
			}
			else _OverrideVisibleUnits.Remove(Unit);
		}

		public void UpdateUnitVisibility(Unit Unit, Tile MovedFrom, Tile MovedTo)
		{
			if (Unit.Army == this || !MovedTo.Configuration.Concealing) return;
			if (CanSeeTile(MovedFrom)) _OverrideVisibleUnits.Add(Unit);
			else _OverrideVisibleUnits.Remove(Unit);
		}

		public void UpdateUnitVisibility(Unit Unit, Tile MovedTo)
		{
			UpdateUnitVisibility(Unit, null, MovedTo);
		}

		void UnitDestroyed(object Sender, EventArgs E)
		{
			Unit unit = (Unit)Sender;
			if (unit.Configuration.IsArmored && unit.Configuration.IsVehicle)
			{
				Unit wreckage = new Unit(this, GameData.Wreckage, _IdGenerator);
				if (OnUnitAdded != null) OnUnitAdded(this, new NewUnitEventArgs(wreckage));
				wreckage.Place(unit.Position);
			}
		}

		public override string ToString()
		{
			return string.Format(
				"[Army: Id={0}, Faction.Name={1} ({2})]",
				Id,
				Configuration.Faction.Name,
				Configuration.Faction.UniqueKey);
		}
	}
}
