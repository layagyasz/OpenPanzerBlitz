using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class RandomSequence : Sequence
	{
		readonly byte[] _Values;

		public int Count
		{
			get
			{
				return _Values.Length;
			}
		}

		public RandomSequence(IEnumerable<byte> Values)
		{
			_Values = Values.ToArray();
		}

		public RandomSequence(ParseBlock Block)
		{
			_Values = Parse.Array(Block.String, Convert.ToByte);
		}

		public RandomSequence(SerializationInputStream Stream)
			: this(Stream.ReadEnumerable(i => i.ReadByte()).ToArray()) { }

		public IEnumerable<byte> Get(Random Random, int Repetitions)
		{
			for (int i = 0; i < Repetitions; ++i)
			{
				byte[] _subsequence = _Values.OrderBy(j => Random.Next()).ToArray();
				foreach (var b in _subsequence) yield return b;
			}
		}

		public Sequence MakeStatic(Random Random, int Repetitions)
		{
			return new StaticSequence(_Values.Length, Get(Random, Repetitions));
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_Values, i => Stream.Write(i));
		}
	}
}
