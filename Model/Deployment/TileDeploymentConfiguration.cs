using System;
using System.Collections.Generic;
using System.Linq;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { UNIT_GROUP, COORDINATE }

		public UnitGroup UnitGroup { get; }
		public readonly Coordinate Coordinate;

		public TileDeploymentConfiguration(UnitGroup UnitGroup, Coordinate Coordinate)
		{
			this.UnitGroup = UnitGroup;
			this.Coordinate = Coordinate;
		}

		public TileDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitGroup = (UnitGroup)attributes[(int)Attribute.UNIT_GROUP];
			Coordinate = (Coordinate)attributes[(int)Attribute.COORDINATE];
		}

		public TileDeploymentConfiguration(SerializationInputStream Stream)
			: this(new UnitGroup(Stream), new Coordinate(Stream.ReadInt32(), Stream.ReadInt32())) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitGroup);
			Stream.Write(Coordinate.X);
			Stream.Write(Coordinate.Y);
		}

		public Deployment GenerateDeployment(Army Army, IdGenerator IdGenerator)
		{
			return new TileDeployment(
				Army, UnitGroup.GenerateUnits(Army, IdGenerator), this, IdGenerator);
		}	}
}
