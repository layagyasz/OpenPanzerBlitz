using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class Army : GameObject
	{
		public EventHandler<NewUnitEventArgs> OnUnitAdded;

		public readonly Match Match;
		public readonly SightFinder SightFinder;
		public readonly ArmyConfiguration Configuration;
		public readonly List<Deployment> Deployments;

		int _Id;
		IdGenerator _IdGenerator;

		List<Unit> _CapturedUnits = new List<Unit>();
		HashSet<Tile> _AttackedTiles = new HashSet<Tile>();

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
				return Deployments.SelectMany(i => i.Units).Concat(_CapturedUnits);
			}
		}

		public Army(Match Match, SightFinder SightFinder, ArmyConfiguration ArmyConfiguration, IdGenerator IdGenerator)
		{
			_Id = IdGenerator.GenerateId();
			this.Match = Match;
			this.SightFinder = SightFinder;

			Configuration = ArmyConfiguration;
			Deployments = ArmyConfiguration.DeploymentConfigurations.Select(
				i => i.GenerateDeployment(this, IdGenerator)).ToList();

			Match.Relay.OnUnitDestroy += UnitDestroyed;
			Match.Relay.OnUnitCapture += UnitCaptured;

			SightFinder.TrackingArmy = this;
			SightFinder.Hook(Match.Relay);
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

		void UnitDestroyed(object Sender, EventArgs E)
		{
			var unit = (Unit)Sender;
			if (unit.Army != this) return;

			if (unit.Configuration.LeavesWreckWhenDestroyed)
			{
				var wreckage = new Unit(this, GameData.Wreckage, _IdGenerator);
				if (OnUnitAdded != null) OnUnitAdded(this, new NewUnitEventArgs(wreckage));
				wreckage.Place(unit.Position);
			}
		}

		void UnitCaptured(object Sender, ValuedEventArgs<Army> E)
		{
			Unit unit = (Unit)Sender;
			if (unit.Army != this) return;

			var newUnit = new Unit(this, unit.Configuration, _IdGenerator);
			newUnit.OnDestroy += UnitDestroyed;
			newUnit.OnCapture += UnitCaptured;
			_CapturedUnits.Add(newUnit);
			if (OnUnitAdded != null) OnUnitAdded(this, new NewUnitEventArgs(newUnit));
			newUnit.Place(unit.Position);
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
