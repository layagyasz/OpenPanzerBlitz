﻿using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class MatchLobby : Serializable
	{
		public EventHandler<ValuedEventArgs<LobbyAction>> OnActionApplied;
		public EventHandler<ValuedEventArgs<Scenario>> OnLaunched;

		Scenario _Scenario;
		List<Player> _Players;
		Dictionary<Player, ArmyConfiguration> _PlayerArmies;
		Dictionary<Player, bool> _PlayerReady;

		public IEnumerable<Player> Players
		{
			get
			{
				return _Players;
			}
		}
		public Scenario Scenario
		{
			get
			{
				return _Scenario;
			}
		}

		public MatchLobby(Scenario Scenario)
		{
			_Scenario = Scenario;
			_Players = new List<Player>();
			_PlayerArmies = new Dictionary<Player, ArmyConfiguration>();
			_PlayerReady = new Dictionary<Player, bool>();
		}

		public MatchLobby(SerializationInputStream Stream)
		{
			_Scenario = new Scenario(Stream);
			_Players = Stream.ReadEnumerable(i => new Player(Stream)).ToList();
			_PlayerArmies = Stream.ReadEnumerable(
				i => new KeyValuePair<Player, ArmyConfiguration>(
					_Players[Stream.ReadByte()], ReadArmyConfiguration(Stream)))
								  .ToDictionary(i => i.Key, i => i.Value);
			_PlayerReady = Stream.ReadEnumerable(
				i => new KeyValuePair<Player, bool>(
					_Players[Stream.ReadByte()], Stream.ReadBoolean())).ToDictionary(i => i.Key, i => i.Value);
		}

		public bool ApplyAction(LobbyAction Action)
		{
			var r = Action.Apply(this);
			if (r && OnActionApplied != null) OnActionApplied(this, new ValuedEventArgs<LobbyAction>(Action));
			return r;
		}

		public bool SetScenario(Scenario Scenario)
		{
			if (_Scenario != Scenario)
			{
				_Scenario = Scenario;
				foreach (Player p in _Players)
				{
					_PlayerArmies[p] = null;
					_PlayerReady[p] = false;
				}
				return true;
			}
			return false;
		}

		public bool AddPlayer(Player Player)
		{
			_Players.Add(Player);
			_PlayerArmies.Add(Player, null);
			_PlayerReady.Add(Player, false);
			return true;
		}

		public ArmyConfiguration GetPlayerArmy(Player Player)
		{
			return _PlayerArmies[Player];
		}

		public bool SetPlayerArmy(Player Player, ArmyConfiguration Army)
		{
			if (_Players.Contains(Player) && !_PlayerReady[Player])
			{
				_PlayerArmies[Player] = Army;
				return true;
			}
			return false;
		}

		public bool GetPlayerReady(Player Player)
		{
			return _PlayerReady[Player];
		}

		public bool SetPlayerReady(Player Player, bool Ready)
		{
			if (_Players.Contains(Player))
			{
				_PlayerReady[Player] = Ready;
				return true;
			}
			return false;
		}

		public bool Start(Scenario StaticScenario)
		{
			// All players are in the ready state.
			if (!_PlayerReady.Values.All(i => i)) return false;

			var pickedArmies = _PlayerArmies.Values.Where(i => i != null).ToList();
			// All players have picked a unique army.
			if (pickedArmies.Count != pickedArmies.Distinct().Count()) return false;
			// All armies are picked.
			if (!_Scenario.ArmyConfigurations.All(i => pickedArmies.Contains(i))) return false;

			if (OnLaunched != null) OnLaunched(this, new ValuedEventArgs<Scenario>(StaticScenario));
			return true;
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_Scenario);
			Stream.Write(_Players);
			Stream.Write(_PlayerArmies, i =>
			{
				Stream.Write((byte)_Players.IndexOf(i.Key));
				Stream.Write((byte)(i.Value == null ? 0 : _Scenario.ArmyConfigurations.IndexOf(i.Value) + 1));
			});
			Stream.Write(_PlayerReady, i =>
			{
				Stream.Write((byte)_Players.IndexOf(i.Key));
				Stream.Write(i.Value);
			});
		}

		ArmyConfiguration ReadArmyConfiguration(SerializationInputStream Stream)
		{
			int index = Stream.ReadByte();
			if (index == 0) return null;
			return _Scenario.ArmyConfigurations[index - 1];
		}
	}
}
