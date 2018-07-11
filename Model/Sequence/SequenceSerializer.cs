using System;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class SequenceSerializer : SerializableAdapter
	{
		public static readonly SequenceSerializer Instance = new SequenceSerializer();

		SequenceSerializer() : base(new Type[] { typeof(RandomSequence), typeof(StaticSequence) }) { }
	}
}
