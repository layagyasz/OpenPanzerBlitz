using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TurnConfiguration : Serializable
	{
		static IEnumerable<TurnComponent> TurnComponents()
		{
			yield return TurnComponent.WAIT;
			yield return TurnComponent.MINEFIELD_ATTACK;
			yield return TurnComponent.ARTILLERY;
			yield return TurnComponent.ATTACK;
			yield return TurnComponent.AIRCRAFT;
			yield return TurnComponent.ANTI_AIRCRAFT;
			yield return TurnComponent.VEHICLE_COMBAT_MOVEMENT;
			yield return TurnComponent.VEHICLE_MOVEMENT;
			yield return TurnComponent.CLOSE_ASSAULT;
			yield return TurnComponent.NON_VEHICLE_MOVEMENT;
			yield return TurnComponent.RESET;
		}

		static IEnumerable<TurnInfo> StandardTurnOrder(IEnumerable<Army> Armies)
		{
			return Armies.SelectMany(i => TurnComponents().Select(j => new TurnInfo(i, j)));
		}

		enum Attribute { TURNS, DEPLOYMENT_ORDER, TURN_ORDER };

		public byte Turns { get; }
		public Sequence DeploymentOrder { get; }
		public Sequence TurnOrder { get; }

		public TurnConfiguration(byte Turns, Sequence DeployOrder, Sequence PlayerOrder)
		{
			this.Turns = Turns;
			this.DeploymentOrder = DeployOrder;
			this.TurnOrder = PlayerOrder;
		}

		public TurnConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Turns = (byte)attributes[(int)Attribute.TURNS];
			DeploymentOrder = (Sequence)attributes[(int)Attribute.DEPLOYMENT_ORDER];
			TurnOrder = (Sequence)attributes[(int)Attribute.TURN_ORDER];
		}

		public TurnConfiguration(SerializationInputStream Stream)
			: this(
				Stream.ReadByte(),
				(Sequence)SequenceSerializer.Instance.Deserialize(Stream),
				(Sequence)SequenceSerializer.Instance.Deserialize(Stream))
		{ }

		public IEnumerable<Turn> Materialize(Random Random, IEnumerable<Army> Armies)
		{
			var armies = Armies.ToArray();
			var deployOrder = DeploymentOrder.Get(Random, 1).Select(i => armies[i]);
			foreach (var army in deployOrder) yield return new Turn(0, new TurnInfo(army, TurnComponent.DEPLOYMENT));
			foreach (var army in deployOrder) yield return new Turn(0, new TurnInfo(army, TurnComponent.RESET));

			var playerOrder = TurnOrder.Get(Random, Turns).Select(i => armies[i]).GetEnumerator();
			for (int i = 0; i < Turns; ++i)
			{
				for (int j = 0; j < TurnOrder.Count; ++j)
				{
					playerOrder.MoveNext();
					foreach (var turnComponent in TurnComponents())
						yield return new Turn((byte)(i + 1), new TurnInfo(playerOrder.Current, turnComponent));
				}
			}
		}

		public TurnConfiguration MakeStatic(Random Random)
		{
			return new TurnConfiguration(
				Turns, DeploymentOrder.MakeStatic(Random, Turns), TurnOrder.MakeStatic(Random, Turns));
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Turns);
			SequenceSerializer.Instance.Serialize(DeploymentOrder, Stream);
			SequenceSerializer.Instance.Serialize(TurnOrder, Stream);
		}
	}
}
