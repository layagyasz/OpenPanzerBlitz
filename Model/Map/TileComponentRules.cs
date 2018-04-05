using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileComponentRules : Serializable
	{
		enum Attribute
		{
			DIE_MODIFIER,
			ROAD_MOVE,
			TREAT_UNITS_AS_ARMORED,
			MUST_ATTACK_ALL_UNITS,

			OVERRIDE_BASE_MOVEMENT,
			DEPRESSED_TRANSITION,
			BLOCKS_LINE_OF_SIGHT,
			CONCEALING,
			LOW_PROFILE_CONCEALING,

			TERRAIN_ATTRIBUTES
		};

		public readonly string UniqueKey;

		public readonly int DieModifier;
		public readonly bool RoadMove;
		public readonly bool TreatUnitsAsArmored;
		public readonly bool MustAttackAllUnits;

		public readonly bool OverrideBaseMovement;
		public readonly bool DepressedTransition;
		public readonly bool BlocksLineOfSight;
		public readonly bool Concealing;
		public readonly bool LowProfileConcealing;

		bool[] _TerrainAttributes;

		public TileComponentRules(
			string UniqueKey,

			int DieModifier,
			bool RoadMove,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits,

			bool OverrideBaseMovement,
			bool DepressedTransition,
			bool BlocksLineOfSight,
			bool Concealing,
			bool LowProfileConcealing,

			IEnumerable<TerrainAttribute> TerrainAttributes)
		{
			this.UniqueKey = UniqueKey;

			this.DieModifier = DieModifier;
			this.RoadMove = RoadMove;
			this.TreatUnitsAsArmored = TreatUnitsAsArmored;
			this.MustAttackAllUnits = MustAttackAllUnits;

			this.OverrideBaseMovement = OverrideBaseMovement;
			this.DepressedTransition = DepressedTransition;
			this.BlocksLineOfSight = BlocksLineOfSight;
			this.Concealing = Concealing;
			this.LowProfileConcealing = LowProfileConcealing;

			_TerrainAttributes = new bool[Enum.GetValues(typeof(TerrainAttribute)).Length];
			foreach (TerrainAttribute attribute in TerrainAttributes) _TerrainAttributes[(int)attribute] = true;
		}

		public TileComponentRules(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),

			   	Stream.ReadInt32(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),

				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Enumerable.Empty<TerrainAttribute>())
		{
			_TerrainAttributes = Stream.ReadEnumerable(Stream.ReadBoolean).ToArray();
		}

		public TileComponentRules(ParseBlock Block)
		{
			UniqueKey = Block.Name;

			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			DieModifier = (int)(attributes[(int)Attribute.DIE_MODIFIER] ?? 0);
			RoadMove = (bool)(attributes[(int)Attribute.ROAD_MOVE] ?? false);
			TreatUnitsAsArmored = (bool)(attributes[(int)Attribute.TREAT_UNITS_AS_ARMORED] ?? false);
			MustAttackAllUnits = (bool)(attributes[(int)Attribute.MUST_ATTACK_ALL_UNITS] ?? false);

			OverrideBaseMovement = (bool)(attributes[(int)Attribute.OVERRIDE_BASE_MOVEMENT] ?? true);
			DepressedTransition = (bool)(attributes[(int)Attribute.DEPRESSED_TRANSITION] ?? false);
			BlocksLineOfSight = (bool)(attributes[(int)Attribute.BLOCKS_LINE_OF_SIGHT] ?? false);
			Concealing = (bool)(attributes[(int)Attribute.CONCEALING] ?? false);
			LowProfileConcealing = (bool)(attributes[(int)Attribute.LOW_PROFILE_CONCEALING] ?? false);

			_TerrainAttributes = new bool[Enum.GetValues(typeof(TerrainAttribute)).Length];
			foreach (TerrainAttribute attribute in
					 (IEnumerable<TerrainAttribute>)(attributes[(int)Attribute.TERRAIN_ATTRIBUTES]
													 ?? Enumerable.Empty<TerrainAttribute>()))
			{
				_TerrainAttributes[(int)attribute] = true;
			}
		}

		public bool HasAttribute(TerrainAttribute Attribute)
		{
			return _TerrainAttributes[(int)Attribute];
		}

		public IEnumerable<TerrainAttribute> GetAttributes()
		{
			return Enum.GetValues(typeof(TerrainAttribute)).Cast<TerrainAttribute>().Where(HasAttribute);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);

			Stream.Write(DieModifier);
			Stream.Write(RoadMove);
			Stream.Write(TreatUnitsAsArmored);
			Stream.Write(MustAttackAllUnits);

			Stream.Write(OverrideBaseMovement);
			Stream.Write(DepressedTransition);
			Stream.Write(BlocksLineOfSight);
			Stream.Write(Concealing);
			Stream.Write(LowProfileConcealing);

			Stream.Write(_TerrainAttributes, Stream.Write);
		}
	}
}
