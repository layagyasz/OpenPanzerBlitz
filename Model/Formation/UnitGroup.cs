using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitGroup : Formation, Serializable
	{
		enum Attribute { NAME, UNIT_COUNTS }

		public string Name { get; }

		List<UnitCount> _UnitCounts;

		public IEnumerable<UnitCount> UnitCounts
		{
			get { return _UnitCounts; }
		}

		public IEnumerable<UnitConfiguration> UnitConfigurations
		{
			get
			{
				return _UnitCounts.SelectMany(i => Enumerable.Repeat(i.UnitConfiguration, i.Count));
			}
		}

		public UnitGroup(string Name, IEnumerable<UnitCount> UnitCounts)
		{
			this.Name = Name;
			_UnitCounts = UnitCounts.ToList();
		}

		public UnitGroup(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Name = (string)attributes[(int)Attribute.NAME];
			_UnitCounts = (List<UnitCount>)attributes[(int)Attribute.UNIT_COUNTS];
		}

		public UnitGroup(SerializationInputStream Stream)
		{
			Name = Stream.ReadString();
			_UnitCounts = Stream.ReadEnumerable(i => new UnitCount(i)).ToList();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Name);
			Stream.Write(_UnitCounts);
		}

		public IEnumerable<Unit> GenerateUnits(Army Army, IdGenerator IdGenerator)
		{
			return UnitConfigurations.Select(i => new Unit(Army, i, IdGenerator));
		}

		public IEnumerable<UnitCount> Flatten()
		{
			return _UnitCounts;
		}
	}
}
