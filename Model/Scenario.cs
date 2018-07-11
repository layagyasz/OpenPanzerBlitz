using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Scenario : Serializable
	{
		enum Attribute
		{
			NAME,
			ENVIRONMENT,
			MAP_CONFIGURATION,
			ARMY_CONFIGURATIONS,
			TURN_CONFIGURATION,
			FOG_OF_WAR
		};

		public readonly string Name;
		public readonly List<ArmyConfiguration> ArmyConfigurations;
		public readonly TurnConfiguration TurnConfiguration;
		public bool FogOfWar { get; set; }
		public readonly Environment Environment;
		public readonly MapConfiguration MapConfiguration;

		public IEnumerable<UnitConfiguration> UnitConfigurations
		{
			get { return ArmyConfigurations.SelectMany(i => i.UnitConfigurations); }
		}

		public Scenario(
			string Name,
			IEnumerable<ArmyConfiguration> ArmyConfigurations,
			TurnConfiguration TurnConfiguration,
			bool FogOfWar,
			Environment Environment,
			MapConfiguration MapConfiguration)
		{
			this.Name = Name;
			this.ArmyConfigurations = ArmyConfigurations.ToList();
			this.TurnConfiguration = TurnConfiguration;
			this.FogOfWar = FogOfWar;
			this.Environment = Environment;
			this.MapConfiguration = MapConfiguration;
		}

		public Scenario(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Name = (string)attributes[(int)Attribute.NAME];
			ArmyConfigurations = (List<ArmyConfiguration>)attributes[(int)Attribute.ARMY_CONFIGURATIONS];

			TurnConfiguration = (TurnConfiguration)attributes[(int)Attribute.TURN_CONFIGURATION];
			FogOfWar = (bool)(attributes[(int)Attribute.FOG_OF_WAR] ?? false);

			Environment = (Environment)attributes[(int)Attribute.ENVIRONMENT];
			MapConfiguration = (MapConfiguration)attributes[(int)Attribute.MAP_CONFIGURATION];
		}

		public Scenario(SerializationInputStream Stream)
		: this(
			Stream.ReadString(),
			Stream.ReadEnumerable(i => new ArmyConfiguration(Stream)).ToList(),
			new TurnConfiguration(Stream),
			Stream.ReadBoolean(),
			GameData.Environments[Stream.ReadString()],
			(MapConfiguration)MapConfigurationSerializer.Instance.Deserialize(Stream))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Name);
			Stream.Write(ArmyConfigurations);
			Stream.Write(TurnConfiguration);
			Stream.Write(FogOfWar);
			Stream.Write(Environment.UniqueKey);
			MapConfigurationSerializer.Instance.Serialize(MapConfiguration, Stream);
		}

		public void SetFogOfWar(bool FogOfWar)
		{
			this.FogOfWar = FogOfWar;
		}

		public Scenario MakeStatic(Random Random)
		{
			return new Scenario(
				Name,
				ArmyConfigurations,
				TurnConfiguration.MakeStatic(Random),
				FogOfWar,
				Environment,
				MapConfiguration.MakeStatic(Random));
		}
	}
}
