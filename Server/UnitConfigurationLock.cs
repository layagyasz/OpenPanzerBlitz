using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfigurationLock : Serializable
	{
		enum Attribute { UNIT_CONFIGURATIONS, RARITY }

		public readonly string UniqueId;
		public readonly float Rarity;
		public readonly List<UnitConfigurationLink> UnitConfigurations;

		public UnitConfigurationLock(
			string UniqueId, float Rarity, IEnumerable<UnitConfigurationLink> UnitConfigurations)
		{
			this.UniqueId = UniqueId;
			this.Rarity = Rarity;
			this.UnitConfigurations = UnitConfigurations.ToList();
		}

		public UnitConfigurationLock(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueId = Block.Name;
			Rarity = (float)attributes[(int)Attribute.RARITY];
			UnitConfigurations = ((List<string>)attributes[(int)Attribute.UNIT_CONFIGURATIONS])
				.Select(i => GameData.UnitConfigurationLinks[i]).ToList();
		}

		public UnitConfigurationLock(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				Stream.ReadFloat(),
				Stream.ReadEnumerable(i => GameData.UnitConfigurationLinks[i.ReadString()]))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueId);
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
			return string.Format("[UnitConfigurationLock: UniqueId={0}]", UniqueId);
		}
	}
}
