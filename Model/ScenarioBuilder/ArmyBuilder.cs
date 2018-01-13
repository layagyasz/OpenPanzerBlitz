using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class ArmyBuilder : GameObject
	{
		public int Id { get; }
		public readonly ArmyParameters Parameters;

		List<UnitConfigurationLink> _Units;

		public IEnumerable<UnitConfigurationLink> Units
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

		public bool Validate(IEnumerable<UnitConfigurationLink> Units)
		{
			if (Units == null) return false;
			return Units.All(i => Parameters.Matches(i));
		}

		public void SetUnits(IEnumerable<UnitConfigurationLink> Units)
		{
			_Units = Units.ToList();
		}
	}
}
