using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ConvoyDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { UNIT_GROUP, MATCHER, IS_STRICT_CONVOY, MOVEMENT_AUTOMATOR, ENTRY_TURN }

		public UnitGroup UnitGroup { get; }
		public readonly bool IsStrictConvoy;
		public readonly Matcher<Tile> Matcher;
		public readonly ConvoyMovementAutomator MovementAutomator;
		public readonly byte EntryTurn;

		public ConvoyDeploymentConfiguration(
			UnitGroup UnitGroup,
			bool IsStrictConvoy,
			Matcher<Tile> Matcher,
			ConvoyMovementAutomator MovementAutomator,
			byte EntryTurn)
		{
			this.UnitGroup = UnitGroup;
			this.IsStrictConvoy = IsStrictConvoy;
			this.Matcher = Matcher;
			this.MovementAutomator = MovementAutomator;
			this.EntryTurn = EntryTurn;
		}

		public ConvoyDeploymentConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UnitGroup = (UnitGroup)attributes[(int)Attribute.UNIT_GROUP];
			IsStrictConvoy = (bool)(attributes[(int)Attribute.IS_STRICT_CONVOY] ?? false);
			MovementAutomator = (ConvoyMovementAutomator)attributes[(int)Attribute.MOVEMENT_AUTOMATOR];
			EntryTurn = (byte)(attributes[(int)Attribute.ENTRY_TURN] ?? (byte)0);

			var m = (Matcher<Tile>)attributes[(int)Attribute.MATCHER];
			var edge = new TileOnEdge(Direction.ANY);

			if (m == null) Matcher = edge;
			else Matcher = new CompositeMatcher<Tile>(new Matcher<Tile>[] { edge, m }, Aggregators.AND);
		}

		public ConvoyDeploymentConfiguration(SerializationInputStream Stream)
			: this(
				new UnitGroup(Stream),
				Stream.ReadBoolean(),
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream),
				Stream.ReadObject(i => new ConvoyMovementAutomator(Stream), true),
				Stream.ReadByte())
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UnitGroup);
			Stream.Write(IsStrictConvoy);
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
			Stream.Write(MovementAutomator, true);
			Stream.Write(EntryTurn);
		}

		public Deployment GenerateDeployment(Army Army, IdGenerator IdGenerator)
		{
			return new ConvoyDeployment(Army, UnitGroup.GenerateUnits(Army, IdGenerator), this, IdGenerator);
		}
	}
}
