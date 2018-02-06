using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasConfiguration : Matcher<Unit>
	{
		enum Attribute { UNIT_CONFIGURATION };

		public readonly UnitConfiguration UnitConfiguration;

		public UnitHasConfiguration(UnitConfiguration UnitConfiguration)
		{
			this.UnitConfiguration = UnitConfiguration;
		}

		public UnitHasConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitConfiguration = (UnitConfiguration)attributes[(int)Attribute.UNIT_CONFIGURATION];
		}

		public UnitHasConfiguration(SerializationInputStream Stream)
			: this(GameData.UnitConfigurations[Stream.ReadString()]) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitConfiguration.UniqueKey);
		}

		public bool Matches(Unit Unit)
		{
			return Unit.Configuration == UnitConfiguration;
		}

		public IEnumerable<Matcher<Unit>> Flatten()
		{
			yield return this;
		}
	}
}
