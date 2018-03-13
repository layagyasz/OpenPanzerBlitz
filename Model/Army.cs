using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Army : GameObject
	{
		public EventHandler<NewUnitEventArgs> OnUnitAdded;

		public readonly Match Match;
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

		public Army(Match Match, ArmyConfiguration ArmyConfiguration, IdGenerator IdGenerator)
		{
			_Id = IdGenerator.GenerateId();
			this.Match = Match;
			Configuration = ArmyConfiguration;
			Deployments = ArmyConfiguration.DeploymentConfigurations.Select(
				i => i.GenerateDeployment(this, IdGenerator)).ToList();
			foreach (Unit u in Units) u.OnDestroy += UnitDestroyed;
			_IdGenerator = IdGenerator;
		}

		public ObjectiveSuccessLevel CheckObjectiveSuccessLevel()
		{
			var l = Configuration.VictoryCondition.GetMatchResult(this, Match, false);
			return l == ObjectiveSuccessLevel.DEFEAT ? ObjectiveSuccessLevel.NONE : l;
		}

		public ObjectiveSuccessLevel GetObjectiveSuccessLevel()
		{
			return Configuration.VictoryCondition.GetMatchResult(this, Match, true);
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
			Deployments.ForEach(i => i.ReassessMatch());
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
			return Units.Any(i => i.MustMove() && i.CanMove(Vehicle, false) == OrderInvalidReason.NONE);
		}

		public bool CanSeeUnit(Unit Unit)
		{
			if (Unit.Position == null) return false;
			if (Unit.Configuration.IsAircraft()) return true;

			bool lowProfile = Unit.Configuration.HasLowProfile
								  || Unit.Position.Units.Any(
									  i => i.Configuration.UnitClass == UnitClass.FORT
									  && i.Army == Unit.Army
									  && i.Configuration.HasLowProfile);
			bool concealed = Unit.Position.Rules.Concealing
								 || (Unit.Position.Rules.LowProfileConcealing && lowProfile);
			return !concealed || CanSpotTile(Unit.Position) || _OverrideVisibleUnits.Contains(Unit);
		}

		public bool CanSeeTile(Tile Tile, bool OverrideConcealment = false)
		{
			if (Tile == null) return false;
			if (Tile.Rules.Concealing && !OverrideConcealment) return CanSpotTile(Tile);
			foreach (Unit u in Units.Where(
				i => i.Status == UnitStatus.ACTIVE && i.Configuration.CanSpot))
			{
				var s = u.GetLineOfSight(Tile);
				if (s != null && s.Validate() == NoLineOfSightReason.NONE && u.Configuration.SpotRange >= s.Range)
					return true;
			}
			return false;
		}

		public bool CanSpotTile(Tile Tile)
		{
			if (Tile == null) return false;
			return Units.Any(
				i => i.Position != null
				&& i.Status == UnitStatus.ACTIVE
				&& i.Configuration.CanSpot
				&& (i.Position == Tile || i.Position.Neighbors().Contains(Tile)));
		}

		public void SetUnitVisibility(Unit Unit, bool Visible)
		{
			if (Unit.Army == this || Unit.Position == null || CanSeeUnit(Unit)) return;

			if (Unit.Position != null && CanSeeTile(Unit.Position, true))
			{
				if (Visible) _OverrideVisibleUnits.Add(Unit);
				else _OverrideVisibleUnits.Remove(Unit);
			}
			else _OverrideVisibleUnits.Remove(Unit);
		}

		public void UpdateUnitVisibility(Unit Unit, Tile MovedFrom, Tile MovedTo)
		{
			if (Unit.Army == this || !MovedTo.Rules.Concealing) return;
			if (CanSeeTile(MovedFrom)) _OverrideVisibleUnits.Add(Unit);
			else _OverrideVisibleUnits.Remove(Unit);
		}

		public void UpdateUnitVisibility(Unit Unit, Tile MovedTo)
		{
			UpdateUnitVisibility(Unit, null, MovedTo);
		}

		void UnitDestroyed(object Sender, EventArgs E)
		{
			var unit = (Unit)Sender;
			if (unit.Configuration.LeavesWreckWhenDestroyed)
			{
				var wreckage = new Unit(this, GameData.Wreckage, _IdGenerator);
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
