using System;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ConvoyMovementAutomator : Serializable
	{
		enum Attribute { ORIGIN, DESTINATION, SPEED, STOP_CONDITION, RECIPROCAL_STOP };

		public readonly Coordinate Origin;
		public readonly Coordinate Destination;
		public readonly byte Speed;
		public readonly Matcher<Tile> StopCondition;
		public readonly bool ReciprocalStop;

		public ConvoyMovementAutomator(
			Coordinate Origin,
			Coordinate Destination,
			byte Speed,
			Matcher<Tile> StopCondition,
			bool ReciprocalStop = false)
		{
			this.Origin = Origin;
			this.Destination = Destination;
			this.Speed = Speed;
			this.StopCondition = StopCondition;
			this.ReciprocalStop = ReciprocalStop;
		}

		public ConvoyMovementAutomator(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Origin = (Coordinate)attributes[(int)Attribute.ORIGIN];
			Destination = (Coordinate)attributes[(int)Attribute.DESTINATION];
			Speed = (byte)attributes[(int)Attribute.SPEED];
			StopCondition = (Matcher<Tile>)attributes[(int)Attribute.STOP_CONDITION];
			ReciprocalStop = (bool)(attributes[(int)Attribute.RECIPROCAL_STOP] ?? false);
		}

		public ConvoyMovementAutomator(SerializationInputStream Stream)
			: this(
				new Coordinate(Stream),
				new Coordinate(Stream),
				Stream.ReadByte(),
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream),
				Stream.ReadBoolean())
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Origin);
			Stream.Write(Destination);
			Stream.Write(Speed);
			MatcherSerializer.Instance.Serialize(StopCondition, Stream);
			Stream.Write(ReciprocalStop);
		}

		public bool StopEarly(Army Army)
		{
			if (ReciprocalStop)
			{
				if (Army.Match.Armies.Any(
					a => a.Configuration.Team != Army.Configuration.Team
						&& a.Units.Any(u => u.Position != null && !u.MustMove())))
					return true;
			}
			return false;
		}

		public bool AutomateMovement(Unit Unit, bool Halted)
		{
			if (StopEarly(Unit.Army)) return true;

			var path = new Path<Tile>(
				Unit.Army.Match.Map.Tiles[Origin.X, Origin.Y],
				Unit.Army.Match.Map.Tiles[Destination.X, Destination.Y],
				t => true,
				(t, j) =>
				{
					var rules = t.GetPathOverlayRules(j);
					if (rules == null || !rules.RoadMove) return float.MaxValue;
					return 1;
				},
				(t, j) => t.HeuristicDistanceTo(j),
				t => t.Neighbors(),
				(t, j) => t == j);

			int i = 0;
			for (; i < path.Count; ++i)
			{
				if (path[i] == Unit.Position) break;
			}

			for (int d = 0; i < path.Count && d < Speed; ++i, ++d)
			{
				if (StopCondition.Matches(path[i]))
				{
					Halted = true;
					break;
				}
				if (i < path.Count - 1 && path[i + 1].Units.Count() > 0) break;
			}

			var o = new MovementOrder(Unit, path[i], false, !Halted);
			if (Unit.Army.Match.ExecuteOrder(o) != OrderInvalidReason.NONE)
				throw new Exception(string.Format("Could not move unit {0} to {1}: {2}", Unit, path[i], o.Validate()));
			return Halted;
		}
	}
}
