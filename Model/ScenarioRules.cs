using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public struct ScenarioRules : Serializable
	{
		enum Attribute { FOG_OF_WAR, ALLOW_CLOSE_ASSAULT_CAPTURE }

		public readonly bool FogOfWar;
		public readonly bool AllowCloseAssaultCapture;

		public ScenarioRules(bool FogOfWar, bool AllowCloseAssaultCapture)
		{
			this.FogOfWar = FogOfWar;
			this.AllowCloseAssaultCapture = AllowCloseAssaultCapture;
		}

		public ScenarioRules(SerializationInputStream Stream)
			: this(Stream.ReadBoolean(), Stream.ReadBoolean()) { }

		public ScenarioRules(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			FogOfWar = (bool)(attributes[(int)Attribute.FOG_OF_WAR] ?? false);
			AllowCloseAssaultCapture = (bool)(attributes[(int)Attribute.ALLOW_CLOSE_ASSAULT_CAPTURE] ?? false);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(FogOfWar);
			Stream.Write(AllowCloseAssaultCapture);
		}

		public static ScenarioRules operator &(ScenarioRules a, ScenarioRules b)
		{
			return new ScenarioRules(
				a.FogOfWar && b.FogOfWar, a.AllowCloseAssaultCapture && b.AllowCloseAssaultCapture);
		}

		public static ScenarioRules operator |(ScenarioRules a, ScenarioRules b)
		{
			return new ScenarioRules(
				a.FogOfWar || b.FogOfWar, a.AllowCloseAssaultCapture || b.AllowCloseAssaultCapture);
		}
	}
}
