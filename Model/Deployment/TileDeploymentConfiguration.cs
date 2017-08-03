using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { DISPLAY_NAME, COORDINATE }

		public readonly Coordinate Coordinate;
		string _DisplayName;

		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
		}

		public TileDeploymentConfiguration(string DisplayName, Coordinate Coordinate)
		{
			_DisplayName = DisplayName;
			this.Coordinate = Coordinate;
		}

		public TileDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_DisplayName = (string)attributes[(int)Attribute.DISPLAY_NAME];
			Coordinate = (Coordinate)attributes[(int)Attribute.COORDINATE];
		}

		public TileDeploymentConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadString(), new Coordinate(Stream.ReadInt32(), Stream.ReadInt32())) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_DisplayName);
			Stream.Write(Coordinate.X);
			Stream.Write(Coordinate.Y);
		}

		public Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
		{
			return new TileDeployment(Army, Units, this, IdGenerator);
		}	}
}
