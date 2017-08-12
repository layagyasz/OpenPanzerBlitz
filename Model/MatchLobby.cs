using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class MatchLobby : Serializable
	{
		public EventHandler<ValuedEventArgs<LobbyAction>> OnActionApplied;

		Scenario _Scenario;
		List<Player> _Players;
		Dictionary<Player, ArmyConfiguration> _PlayerArmies;

		public IEnumerable<KeyValuePair<Player, ArmyConfiguration>> PlayerConfiguration
		{
			get
			{
				return _PlayerArmies;
			}
		}

		public MatchLobby()
		{
			_Scenario = GameData.Scenarios.First();
			_Players = new List<Player>();
			_PlayerArmies = new Dictionary<Player, ArmyConfiguration>();
		}

		public MatchLobby(SerializationInputStream Stream)
		{
			_Scenario = new Scenario(Stream);
			_Players = Stream.ReadEnumerable(i => new Player(Stream)).ToList();
			_PlayerArmies = Stream.ReadEnumerable(
				i => new KeyValuePair<Player, ArmyConfiguration>(
					_Players[Stream.ReadByte()], _Scenario.ArmyConfigurations[Stream.ReadByte()]))
								  .ToDictionary(i => i.Key, i => i.Value);
		}

		public bool ApplyAction(LobbyAction Action)
		{
			bool r = Action.Apply(this);
			if (r && OnActionApplied != null) OnActionApplied(this, new ValuedEventArgs<LobbyAction>(Action));
			return r;
		}

		public bool SetScenario(Scenario Scenario)
		{
			_Scenario = Scenario;
			foreach (Player p in _Players) _PlayerArmies[p] = _Scenario.ArmyConfigurations.First();
			return true;
		}

		public bool AddPlayer(Player Player)
		{
			_Players.Add(Player);
			_PlayerArmies.Add(Player, _Scenario == null ? null : _Scenario.ArmyConfigurations.First());
			return true;
		}

		public bool SetArmyPlayer(Player Player, ArmyConfiguration Army)
		{
			if (_Players.Contains(Player))
			{
				_PlayerArmies[Player] = Army;
				return true;
			}
			return false;
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_Scenario);
			Stream.Write(_Players);
			Stream.Write(_PlayerArmies, i =>
			{
				Stream.Write((byte)_Players.IndexOf(i.Key));
				Stream.Write((byte)_Scenario.ArmyConfigurations.IndexOf(i.Value));
			});
		}
	}
}
