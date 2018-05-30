using System.Collections.Generic;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class ConnectionCache<T>
	{
		public bool AllowDuplicateConnections { get; }

		public readonly Dictionary<T, TCPConnection> Connections;

		public ConnectionCache()
		{
			Connections = new Dictionary<T, TCPConnection>();
		}

		public bool PlayerMatches(T Player, TCPConnection Connection)
		{
			return Connections.ContainsValue(Connection) && Connection == Connections[Player];
		}

		public bool CanAdd(T Player, TCPConnection Connection)
		{
			return !Connections.ContainsKey(Player)
								&& (AllowDuplicateConnections || !Connections.ContainsValue(Connection));
		}

		public bool Add(T Player, TCPConnection Connection)
		{
			if (CanAdd(Player, Connection))
			{
				Connections.Add(Player, Connection);
				return true;
			}
			return false;
		}
	}
}
