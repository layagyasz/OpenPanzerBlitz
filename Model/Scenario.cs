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
			RULES
		};

		public readonly string Name;
		public readonly List<ArmyConfiguration> ArmyConfigurations;
		public readonly TurnConfiguration TurnConfiguration;
		public readonly Environment Environment;
		public readonly MapConfiguration MapConfiguration;

		public ScenarioRules Rules { get; set; }

		public IEnumerable<UnitConfiguration> UnitConfigurations
		{
			get { return ArmyConfigurations.SelectMany(i => i.UnitConfigurations); }
		}

		public Scenario(
			string Name,
			IEnumerable<ArmyConfiguration> ArmyConfigurations,
			TurnConfiguration TurnConfiguration,
			Environment Environment,
			MapConfiguration MapConfiguration,
			ScenarioRules Rules)
		{
			this.Name = Name;
			this.ArmyConfigurations = ArmyConfigurations.ToList();
			this.TurnConfiguration = TurnConfiguration;
			this.Environment = Environment;
			this.MapConfiguration = MapConfiguration;
			this.Rules = Rules;
		}

		public Scenario(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Name = (string)attributes[(int)Attribute.NAME];
			ArmyConfigurations = (List<ArmyConfiguration>)attributes[(int)Attribute.ARMY_CONFIGURATIONS];

			TurnConfiguration = (TurnConfiguration)attributes[(int)Attribute.TURN_CONFIGURATION];
			Environment = (Environment)attributes[(int)Attribute.ENVIRONMENT];
			MapConfiguration = (MapConfiguration)attributes[(int)Attribute.MAP_CONFIGURATION];
			Rules = (ScenarioRules)(attributes[(int)Attribute.RULES] ?? default(ScenarioRules));
		}

		public Scenario(SerializationInputStream Stream)
		: this(
			Stream.ReadString(),
			Stream.ReadEnumerable(i => new ArmyConfiguration(Stream)).ToList(),
			new TurnConfiguration(Stream),
			GameData.Environments[Stream.ReadString()],
			(MapConfiguration)MapConfigurationSerializer.Instance.Deserialize(Stream),
			new ScenarioRules(Stream))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Name);
			Stream.Write(ArmyConfigurations);
			Stream.Write(TurnConfiguration);
			Stream.Write(Environment.UniqueKey);
			MapConfigurationSerializer.Instance.Serialize(MapConfiguration, Stream);
			Stream.Write(Rules);
		}

		public Scenario MakeStatic(Random Random)
		{
			return new Scenario(
				Name,
				ArmyConfigurations,
				TurnConfiguration.MakeStatic(Random),
				Environment,
				MapConfiguration.MakeStatic(Random),
				Rules);
		}
	}
}
