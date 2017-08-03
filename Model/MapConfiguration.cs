using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MapConfiguration : Serializable
	{
		public readonly List<List<Tuple<string, bool>>> Boards;

		public MapConfiguration(IEnumerable<IEnumerable<Tuple<string, bool>>> Boards)
		{
			this.Boards = Boards.Select(i => i.ToList()).ToList();
		}

		public MapConfiguration(ParseBlock Block)
		{
			Boards = Block.BreakToList<List<Tuple<object, object>>>().Select(
				i => i.Select(
					j => new Tuple<string, bool>((string)j.Item1, (bool)j.Item2)).ToList()).ToList();
		}

		public MapConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadEnumerable(
				i => Stream.ReadEnumerable(
					j => new Tuple<string, bool>(Stream.ReadString(), Stream.ReadBoolean()))))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Boards, i => Stream.Write(i, j =>
			{
				Stream.Write(j.Item1);
				Stream.Write(j.Item2);
			}));
		}
	}
}
