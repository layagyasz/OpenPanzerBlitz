using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfigurationLock : Serializable
	{
		enum Attribute { UNIT_CONFIGURATIONS, RARITY }

		public readonly string UniqueKey;
		public readonly float Rarity;
		public readonly List<UnitConfigurationLink> UnitConfigurations;

		public UnitConfigurationLock(
			string UniqueKey, float Rarity, IEnumerable<UnitConfigurationLink> UnitConfigurations)
		{
			this.UniqueKey = UniqueKey;
			this.Rarity = Rarity;
			this.UnitConfigurations = UnitConfigurations.ToList();
		}

		public UnitConfigurationLock(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			Rarity = (float)attributes[(int)Attribute.RARITY];
			UnitConfigurations = (List<UnitConfigurationLink>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
		}

		public UnitConfigurationLock(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				Stream.ReadFloat(),
				Stream.ReadEnumerable(i => GameData.UnitConfigurationLinks[i.ReadString()]))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(Rarity);
			Stream.Write(UnitConfigurations, i => Stream.Write(i.UniqueKey));
		}

		public double GetWeight()
		{
			return Math.Pow(2, -Rarity);
		}

		public double GetValue()
		{
			return Math.Pow(2, Rarity);
		}

		public override string ToString()
		{
			return string.Format("[UnitConfigurationLock: UniqueKey={0}]", UniqueKey);
		}
	}
}
