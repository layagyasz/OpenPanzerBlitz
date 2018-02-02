using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class ArmyBuilder : GameObject
	{
		public int Id { get; }
		public readonly ArmyParameters Parameters;

		List<Tuple<UnitConfigurationLink, int>> _Units;

		public IEnumerable<Tuple<UnitConfigurationLink, int>> Units
		{
			get
			{
				return _Units;
			}
		}

		public ArmyBuilder(IdGenerator IdGenerator, ArmyParameters Parameters)
		{
			Id = IdGenerator.GenerateId();
			this.Parameters = Parameters;
		}

		public bool Validate()
		{
			return Validate(_Units);
		}

		public bool Validate(IEnumerable<Tuple<UnitConfigurationLink, int>> Units)
		{
			if (Units == null) return false;
			return Parameters.Matches(Units);
		}

		public bool SetParameters(ArmyParameters Parameters)
		{
			this.Parameters.Copy(Parameters);
			return true;
		}

		public bool SetUnits(IEnumerable<Tuple<UnitConfigurationLink, int>> Units)
		{
			if (Validate(Units))
			{
				_Units = Units.ToList();
				return true;
			}
			return false;
		}

		public ArmyConfiguration BuildArmyConfiguration()
		{
			Objective objective =
				new HighestScoreObjective(
					new UnitsMatchedObjective(new UnitHasStatus(UnitStatus.DESTROYED), false, true, 1));
			return new ArmyConfiguration(
				Id.ToString(),
				Parameters.Faction,
				Parameters.Team,
				Enumerable.Repeat(
					new PositionalDeploymentConfiguration(
						new UnitGroup(
							Parameters.Faction.Name + " Deployment",
							_Units.Select(i => new UnitCount(i.Item1.UnitConfiguration, i.Item2))),
						new EmptyMatcher<Tile>())
					, 1),
				new VictoryCondition(
					Enumerable.Repeat(objective, 1),
					Enumerable.Repeat(
						new ObjectiveSuccessTrigger(ObjectiveSuccessLevel.VICTORY, 1, false, objective), 1)));
		}
	}
}
