using System;

using Cardamom.Serialization;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class Faction
	{
		private enum Attribute { NAME, COLORS, STACK_LIMIT, HALF_PRICE_TRUCKS };

		public readonly string Name;
		public readonly Color[] Colors;
		public readonly byte StackLimit;
		public readonly bool HalfPriceTrucks;

		public Faction(string Name, Color[] Colors, bool HalfPriceTrucks = false)
		{
			this.Name = Name;
			this.Colors = Colors;
			this.HalfPriceTrucks = HalfPriceTrucks;
		}

		public Faction(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Name = (string)attributes[(int)Attribute.NAME];
			Colors = (Color[])attributes[(int)Attribute.COLORS];
			StackLimit = (byte)attributes[(int)Attribute.STACK_LIMIT];
			HalfPriceTrucks = (bool)attributes[(int)Attribute.HALF_PRICE_TRUCKS];
		}
	}
}
