using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Utilities;
using Cardamom.Utilities.StableMatching;

namespace PanzerBlitz
{
	public class UnitAssigner
	{
		public readonly AIRoot Root;

		readonly Dictionary<Unit, List<UnitAssignment>> _Assignments = new Dictionary<Unit, List<UnitAssignment>>();

		public UnitAssigner(AIRoot Root)
		{
			this.Root = Root;
		}

		public void ReAssign()
		{			ClearAssignments();
			foreach (var deployment in Root.Army.Deployments) MakeAssignments(deployment);
		}

		void ClearAssignments()
		{
			_Assignments.Clear();
		}

		void MakeAssignments(Deployment Deployment)
		{
			AssignCarriers(Deployment);
			AssignDefenders(Deployment);
		}

		public IEnumerable<UnitAssignment> GetAssignments(Unit Unit)
		{
			if (_Assignments.ContainsKey(Unit)) return _Assignments[Unit];
			return Enumerable.Empty<UnitAssignment>();
		}

		public IEnumerable<UnitAssignment> GetAssignments()
		{
			return _Assignments.Values.SelectMany(i => i).Distinct();
		}

		void AddAssignment(Unit Unit, UnitAssignment Assignment)
		{
			if (_Assignments.ContainsKey(Unit)) _Assignments[Unit].Add(Assignment);
			else _Assignments.Add(Unit, new List<UnitAssignment> { Assignment });
		}

		void AddAssignment(UnitAssignment Assignment)
		{
			AddAssignment(Assignment.Object, Assignment);
			AddAssignment(Assignment.Subject, Assignment);
		}

		void AssignCarriers(Deployment Deployment)
		{
			var passengers = Deployment.Units.Where(i => i.Configuration.IsPassenger).ToList();
			var carriers = Deployment.Units.Where(i => i.Configuration.IsCarrier).ToList();
			if (passengers.Count == 0 || carriers.Count == 0) return;
			carriers.Sort(new FluentComparator<Unit>(i => i.Configuration.UnitClass == UnitClass.TRANSPORT).Invert());
			carriers = carriers.Take(passengers.Count).ToList();

			var matching = new StableMatching<Unit, Unit>();
			carriers.ForEach(matching.AddPrimaryActor);
			passengers.ForEach(matching.AddSecondaryActor);

			foreach (var passenger in passengers)
			{
				foreach (var carrier in carriers)
				{
					if (carrier.Configuration.CanLoad(passenger.Configuration) == OrderInvalidReason.NONE)
					{
						matching.SetPrimaryPreference(
							carrier,
							passenger,
							passenger.GetPointValue() / Math.Max(.01, passenger.Configuration.Movement));
						matching.SetSecondaryPreference(
							passenger,
							carrier,
							carrier.Configuration.UnitClass == UnitClass.TRANSPORT ? carrier.GetPointValue() : 1);
					}
					else
					{
						matching.SetPrimaryPreference(carrier, passenger, 0);
						matching.SetSecondaryPreference(passenger, carrier, 0);
					}
				}
			}
			foreach (var assignment in matching.GetPairs()
					 .Where(i => i.Second != null
							&& i.First.Configuration.CanLoad(i.Second.Configuration) == OrderInvalidReason.NONE))
				AddAssignment(new UnitAssignment(assignment.First, assignment.Second, UnitAssignmentType.CARRIER));
		}

		void AssignDefenders(Deployment Deployment)
		{
			var guns = Deployment.Units.Where(i => i.Configuration.UnitClass == UnitClass.TOWED_GUN).ToList();
			var defenders = Deployment.Units.Where(
							i => i.Configuration.UnitClass == UnitClass.INFANTRY).ToList();
			if (guns.Count == 0 || defenders.Count == 0) return;
			guns.Sort(new FluentComparator<Unit>(i => i.GetPointValue()).Invert());
			guns = guns.Take(defenders.Count).ToList();

			var matching = new StableMatching<Unit, Unit>();
			guns.ForEach(matching.AddPrimaryActor);
			defenders.ForEach(matching.AddSecondaryActor);

			foreach (var gun in guns)
			{
				foreach (var infantry in defenders)
				{
					matching.SetPrimaryPreference(gun, infantry, infantry.GetPointValue());
					matching.SetSecondaryPreference(infantry, gun, gun.GetPointValue() / gun.Configuration.Defense);
				}
			}
			foreach (var assignment in matching.GetPairs().Where(i => i.Second != null))
				AddAssignment(new UnitAssignment(assignment.Second, assignment.First, UnitAssignmentType.DEFENDER));
		}

		public override string ToString()
		{
			return string.Format(
				"[UnitAssigner: Assignments=`{0}`]",
				string.Join(", ", _Assignments.Values.SelectMany(i => i).Distinct().Select(i => i.ToString())));
		}
	}
}
