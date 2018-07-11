using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class StaticSequence : Sequence
	{
		readonly byte[] _Sequence;

		public int Count { get; }

		public StaticSequence(int Count, IEnumerable<byte> Sequence)
		{
			this.Count = Count;
			_Sequence = Sequence.ToArray();
		}

		public StaticSequence(ParseBlock Block)
		{
			_Sequence = Parse.Array(Block.String, Convert.ToByte);
			Count = _Sequence.Length;
		}

		public StaticSequence(SerializationInputStream Stream)
			: this(Stream.ReadInt32(), Stream.ReadEnumerable(i => i.ReadByte()).ToArray()) { }

		public IEnumerable<byte> Get(Random Random, int Repetitions)
		{
			return Enumerable.Repeat(_Sequence, Repetitions).SelectMany(i => i);
		}

		public Sequence MakeStatic(Random Random, int Repetitions)
		{
			return this;
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Count);
			Stream.Write(_Sequence, i => Stream.Write(i));
		}
	}
}
