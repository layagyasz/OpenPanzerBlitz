using System.Linq;

using Cardamom.Serialization;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class Faction : Serializable
	{
		enum Attribute { NAME, COLORS, STACK_LIMIT, HALF_PRICE_TRUCKS };

		public readonly string UniqueKey;
		public readonly string Name;
		public readonly Color[] Colors;
		public readonly byte StackLimit;
		public readonly bool HalfPriceTrucks;

		public Faction(string UniqueKey, string Name, Color[] Colors, byte StackLimit, bool HalfPriceTrucks = false)
		{
			this.UniqueKey = UniqueKey;
			this.Name = Name;
			this.Colors = Colors;
			this.StackLimit = StackLimit;
			this.HalfPriceTrucks = HalfPriceTrucks;
		}

		public Faction(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				Stream.ReadString(),
				Stream.ReadEnumerable(FileUtils.DeserializeColor).ToArray(),
				Stream.ReadByte(),
				Stream.ReadBoolean())
		{ }

		public Faction(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			Name = (string)attributes[(int)Attribute.NAME];
			Colors = (Color[])attributes[(int)Attribute.COLORS];
			StackLimit = (byte)attributes[(int)Attribute.STACK_LIMIT];
			HalfPriceTrucks = Parse.DefaultIfNull(attributes[(int)Attribute.HALF_PRICE_TRUCKS], false);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(Name);
			Stream.Write(Colors, i => FileUtils.SerializeColor(Stream, i));
			Stream.Write(StackLimit);
			Stream.Write(HalfPriceTrucks);
		}
	}
}
