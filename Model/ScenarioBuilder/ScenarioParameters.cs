using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ScenarioParameters : Serializable
	{
		public uint Year { get; private set; }
		public MatchSetting Setting { get; private set; }

		public byte Turns { get; private set; }
		public Coordinate MapSize { get; private set; }
		public bool FogOfWar { get; private set; }

		public ScenarioParameters(
			uint Year, MatchSetting Setting, byte Turns, Coordinate MapSize, bool FogOfWar)
		{
			this.Year = Year;
			this.Setting = Setting;

			this.Turns = Turns;
			this.MapSize = MapSize;
			this.FogOfWar = FogOfWar;
		}

		public ScenarioParameters(SerializationInputStream Stream)
			: this(
				Stream.ReadUInt32(),
				GameData.MatchSettings[Stream.ReadString()],
				Stream.ReadByte(),
				new Coordinate(Stream),
				Stream.ReadBoolean())
		{ }

		public void Copy(ScenarioParameters Parameters)
		{
			Year = Parameters.Year;
			Setting = Parameters.Setting;
			Turns = Parameters.Turns;
			MapSize = Parameters.MapSize;
			FogOfWar = Parameters.FogOfWar;
		}

		public bool Matches(UnitConstraints Constraints)
		{
			if (Setting.Front != Front.ALL && Constraints.Front != Front.ALL && Setting.Front != Constraints.Front)
				return false;
			if (Constraints.IntroduceYear > 0 && Year < Constraints.IntroduceYear)
				return false;
			if (Constraints.ObsoleteYear > 0 && Year > Constraints.ObsoleteYear)
				return false;
			if (Constraints.Environments.Count > 0 && !Constraints.Environments.Contains(Setting.Environment))
				return false;
			return true;
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Year);
			Stream.Write(Setting.UniqueKey);
			Stream.Write(Turns);
			Stream.Write(MapSize);
			Stream.Write(FogOfWar);
		}

		public override string ToString()
		{
			return string.Format(
				"[ScenarioParameters: Year={0}, Setting={1}, Turns={2}, MapSize={3}, FogOfWar={4}]",
				Year,
				Setting,
				Turns,
				MapSize,
				FogOfWar);
		}
	}
}
