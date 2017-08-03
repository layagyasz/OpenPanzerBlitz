using System;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class DistanceFromUnit : Matcher
	{
		enum Attribute { UNIT_CONFIGURATION, DISTANCE, ATLEAST };

		public readonly UnitConfiguration UnitConfiguration;
		public readonly int Distance;
		public readonly bool Atleast;

		public DistanceFromUnit(UnitConfiguration UnitConfiguration, int Distance, bool Atleast)
		{
			this.UnitConfiguration = UnitConfiguration;
			this.Distance = Distance;
			this.Atleast = Atleast;
		}

		public DistanceFromUnit(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitConfiguration = (UnitConfiguration)attributes[(int)Attribute.UNIT_CONFIGURATION];
			Distance = (int)attributes[(int)Attribute.DISTANCE];
			Atleast = Parse.DefaultIfNull(attributes[(int)Attribute.ATLEAST], false);
		}

		public DistanceFromUnit(SerializationInputStream Stream)
			: this(GameData.UnitConfigurations[Stream.ReadString()], Stream.ReadInt32(), Stream.ReadBoolean()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitConfiguration.UniqueKey);
			Stream.Write(Distance);
			Stream.Write(Atleast);
		}

		public bool Matches(Tile Tile)
		{
			return new Field<Tile>(Tile, Distance, (i, j) => 1)
				.GetReachableNodes()
				.Any(i => i.Item1.Units.Any(j => j.Configuration == UnitConfiguration)) ^ Atleast;
		}
	}
}
