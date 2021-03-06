using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cardamom.Planar;

namespace PanzerBlitz
{
	public static class ObjectDescriber
	{
		public static readonly char[] ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToArray();

		public static string Describe(object Object)
		{
			if (Object is Enum) return Describe((Enum)Object);
			if (Object is Unit) return Describe((Unit)Object);
			if (Object is ArmyConfiguration) return Describe((ArmyConfiguration)Object);
			return Object.ToString();
		}

		public static string Describe(Enum EnumValue)
		{
			return Describe(EnumValue.ToString(), '_');
		}

		public static string Describe(Faction Faction)
		{
			return Faction.Name;
		}

		public static string Describe(Army Army)
		{
			return Describe(Army.Configuration);
		}

		public static string Describe(ArmyConfiguration ArmyConfiguration)
		{
			return Describe(ArmyConfiguration.Faction);
		}

		public static string Describe(Unit Unit)
		{
			return string.Format("{0} (#{1})", Describe(Unit.Configuration), Unit.Id);
		}

		public static string Describe(UnitConfiguration Configuration)
		{
			return Configuration.Name;
		}

		public static string Describe(MapRegion Region)
		{
			return Region.Name;
		}

		public static string Describe(Environment Environment)
		{
			return Describe(Environment.UniqueKey, '-');
		}

		public static string Describe(MatchSetting Setting)
		{
			return Describe(Setting.UniqueKey, '-');
		}

		public static string Describe(Coordinate Coordinate)
		{
			return string.Format("({0}, {1})", Coordinate.X, IntToString(Coordinate.Y, ALPHABET));
		}

		public static string Describe(Polygon Zone)
		{
			return string.Format(
				"[{0}]",
				string.Join(", ", Zone.Segments.Select(i => Describe(new Coordinate((int)i.Point.X, (int)i.Point.Y)))));
		}

		public static string Describe(ObjectiveSuccessTrigger Trigger)
		{
			return Sentencify(
				ObjectiveDescriber.Describe(
				new TriggerObjective(Trigger.Objective, Trigger.Threshold, Trigger.Invert)));
		}

		public static string Describe(string Value, char Separator)
		{
			return string.Join(" ", Value.ToLower().Split(Separator).Select(Capitalize));
		}

		public static string Capitalize(string Input)
		{
			if (Input.Length == 0) return Input;
			return char.ToUpper(Input[0]) + Input.Substring(1);
		}

		public static string Sentencify(string Input)
		{
			if (Input.Length == 0) return Input;
			return Capitalize(Input) + ".";
		}

		public static string Namify(string Input)
		{
			return string.Join(string.Empty, Regex.Split(Input, @"(?<=[ -'])").Select(Capitalize));
		}

		public static string Listify(IEnumerable<string> Parts, string Joiner, string TerminalJoiner)
		{
			return string.Format(
				"{0}{1}{2}", string.Join(Joiner, Parts.Take(Parts.Count() - 1)), TerminalJoiner, Parts.Last());
		}

		public static string IntToString(int Value, char[] BaseChars)
		{
			int i = 32;
			char[] buffer = new char[i];
			int targetBase = BaseChars.Length;

			do
			{
				buffer[--i] = BaseChars[Value % targetBase];
				Value = Value / targetBase;
			}
			while (Value > 0);

			char[] result = new char[32 - i];
			Array.Copy(buffer, i, result, 0, 32 - i);

			return new string(result);
		}
	}
}
