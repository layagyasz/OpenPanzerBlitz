using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface Sequence : Serializable
	{
		int Count { get; }
		IEnumerable<byte> Get(Random Random, int Repetitions);
		Sequence MakeStatic(Random Random, int Repetitions);
	}
}
