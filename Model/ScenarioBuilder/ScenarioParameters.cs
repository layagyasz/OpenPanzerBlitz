using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ScenarioParameters : Serializable
	{
		public uint Year { get; private set; }
		public Front Front { get; private set; }
		public Environment Environment { get; private set; }

		public byte Turns { get; private set; }
		public Coordinate MapSize { get; private set; }

		public ScenarioParameters(uint Year, Front Front, Environment Environment, byte Turns, Coordinate MapSize)
		{
			this.Year = Year;
			this.Front = Front;
			this.Environment = Environment;

			this.Turns = Turns;
			this.MapSize = MapSize;
		}

		public ScenarioParameters(SerializationInputStream Stream)
			: this(
				Stream.ReadUInt32(),
				(Front)Stream.ReadByte(),
				GameData.Environments[Stream.ReadString()],
				Stream.ReadByte(),
				new Coordinate(Stream))
		{ }

		public void Copy(ScenarioParameters Parameters)
		{
			Year = Parameters.Year;
			Front = Parameters.Front;
			Environment = Parameters.Environment;
			Turns = Parameters.Turns;
			MapSize = Parameters.MapSize;
		}

		public bool Matches(UnitConfigurationLink Link)
		{
			if (Front != Front.ALL && Link.Front != Front.ALL && Front != Link.Front) return false;
			if (Link.IntroduceYear > 0 && Year < Link.IntroduceYear) return false;
			if (Link.ObsoleteYear > 0 && Year > Link.ObsoleteYear) return false;
			if (Link.Environments != null && !Link.Environments.Contains(Environment)) return false;
			return true;
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Year);
			Stream.Write((byte)Front);
			Stream.Write(Environment.UniqueKey);
			Stream.Write(Turns);
			Stream.Write(MapSize);
		}

		public override string ToString()
		{
			return string.Format(
				"[ScenarioParameters: Year={0}, Front={1}, Environment={2}, Turns={3}, MapSize={4}]",
				Year,
				Front,
				Environment,
				Turns,
				MapSize);
		}
	}
}
