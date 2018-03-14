using Cardamom.Serialization;

namespace PanzerBlitz
{
	public struct Weapon : Serializable
	{
		enum Attribute { WEAPON_CLASS, ATTACK, RANGE, CAN_DOUBLE_RANGE, AMMUNITION }

		public readonly WeaponClass WeaponClass;
		public readonly byte Attack;
		public readonly byte Range;
		public readonly bool CanDoubleRange;
		public readonly byte Ammunition;

		public Weapon(WeaponClass WeaponClass, byte Attack, byte Range, bool CanDoubleRange, byte Ammunition)
		{
			this.WeaponClass = WeaponClass;
			this.Attack = Attack;
			this.Range = Range;
			this.CanDoubleRange = CanDoubleRange;
			this.Ammunition = Ammunition;
		}

		public Weapon(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			WeaponClass = (WeaponClass)attributes[(int)Attribute.WEAPON_CLASS];
			Attack = (byte)attributes[(int)Attribute.ATTACK];
			Range = (byte)attributes[(int)Attribute.RANGE];
			CanDoubleRange = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_DOUBLE_RANGE], false);
			Ammunition = Parse.DefaultIfNull(attributes[(int)Attribute.AMMUNITION], (byte)0);
		}

		public Weapon(SerializationInputStream Stream)
			: this(
				(WeaponClass)Stream.ReadByte(),
				Stream.ReadByte(),
				Stream.ReadByte(),
				Stream.ReadBoolean(),
				Stream.ReadByte())
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)WeaponClass);
			Stream.Write(Attack);
			Stream.Write(Range);
			Stream.Write(CanDoubleRange);
			Stream.Write(Ammunition);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Weapon)) return false;
			var w = (Weapon)obj;
			return WeaponClass == w.WeaponClass
								   && Attack == w.Attack
								   && Range == w.Range
								   && CanDoubleRange == w.CanDoubleRange
								   && Ammunition == w.Ammunition;
		}

		public override int GetHashCode()
		{
			return WeaponClass.GetHashCode()
							  ^ Attack.GetHashCode()
							  ^ Range.GetHashCode()
							  ^ CanDoubleRange.GetHashCode()
							  ^ Ammunition.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(
				"[Weapon: WeaponClass={0}, Attack={1}, Range={2}, CanDoubleRange={3}, Ammunition={4}]",
				WeaponClass,
				Attack,
				Range,
				CanDoubleRange,
				Ammunition);
		}

		public static bool operator ==(Weapon w1, Weapon w2)
		{
			return w1.Equals(w2);
		}

		public static bool operator !=(Weapon w1, Weapon w2)
		{
			return !w1.Equals(w2);
		}
	}
}
