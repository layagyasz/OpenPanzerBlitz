using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ConvoyMovementAutomator : Serializable
	{
		enum Attribute { DESTINATION, SPEED, STOP_CONDITION };

		public readonly Coordinate Destination;
		public readonly byte Speed;
		public readonly Matcher<Tile> StopCondition;

		Dictionary<Unit, Path<Tile>> _CachedPaths = new Dictionary<Unit, Path<Tile>>();

		public ConvoyMovementAutomator(Coordinate Destination, byte Speed, Matcher<Tile> StopCondition)
		{
			this.Destination = Destination;
			this.Speed = Speed;
			this.StopCondition = StopCondition;
		}

		public ConvoyMovementAutomator(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Destination = (Coordinate)attributes[(int)Attribute.DESTINATION];
			Speed = (byte)attributes[(int)Attribute.SPEED];
			StopCondition = (Matcher<Tile>)attributes[(int)Attribute.STOP_CONDITION];
		}

		public ConvoyMovementAutomator(SerializationInputStream Stream)
			: this(
				new Coordinate(Stream),
				Stream.ReadByte(),
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Destination);
			Stream.Write(Speed);
			Stream.Write(StopCondition);
		}

		Path<Tile> GetPathForUnit(Unit Unit, Match Match)
		{
			if (_CachedPaths.ContainsKey(Unit)) return _CachedPaths[Unit];

			Path<Tile> path = Unit.GetPathTo(Match.Map.Tiles[Destination.X, Destination.Y], false);
			_CachedPaths.Add(Unit, path);
			return path;
		}

		public bool AutomateMovement(Unit Unit, Match Match)
		{
			Path<Tile> path = GetPathForUnit(Unit, Match);
			bool halt = false;

			int i = 0;
			for (; i < path.Count; ++i)
			{
				if (path[i] == Unit.Position) break;
			}

			for (int d = 0; i < path.Count && d <= Speed; ++i, ++d)
			{
				if (StopCondition.Matches(path[i]))
				{
					halt = true;
					break;
				}
				if (i < path.Count - 1 && path[i + 1].Units.Count() > 0) break;
			}

			if (!Match.ExecuteOrder(new MovementOrder(Unit, path[i], false)))
				throw new Exception(string.Format("Could not move unit {0} to {1}", Unit, path[i]));

			return halt;
		}
	}
}
