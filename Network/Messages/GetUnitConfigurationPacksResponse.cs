using System.Collections.Generic;
using System.Linq;

using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class GetUnitConfigurationPacksResponse : RPCResponse
	{
		public readonly List<UnitConfigurationPack> UnitConfigurationPacks;

		public GetUnitConfigurationPacksResponse(IEnumerable<UnitConfigurationPack> UnitConfigurationPacks)
		{
			this.UnitConfigurationPacks = UnitConfigurationPacks.ToList();
		}

		public GetUnitConfigurationPacksResponse(SerializationInputStream Stream)
			: base(Stream)
		{
			UnitConfigurationPacks = Stream.ReadEnumerable(i => new UnitConfigurationPack(i)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(UnitConfigurationPacks);
		}
	}
}
