using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitMovementRules
	{
		enum Attribute
		{
			DENSE_EDGE,
			DEPRESSED,
			ROUGH,
			SLOPED,
			SWAMP,
			WATER,

			IGNORES_ENVIRONMENT_MOVEMENT,
			CANNOT_USE_ROAD_MOVEMENT
		};

		public readonly BlockType DenseEdge;
		public readonly BlockType Depressed;
		public readonly BlockType Rough;
		public readonly BlockType Sloped;
		public readonly BlockType Swamp;
		public readonly BlockType Water;

		public readonly bool IgnoresEnvironmentMovement;
		public readonly bool CannotUseRoadMovement;

		public UnitMovementRules(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			DenseEdge = Parse.DefaultIfNull(attributes[(int)Attribute.DENSE_EDGE], BlockType.CLEAR);
			Depressed = Parse.DefaultIfNull(attributes[(int)Attribute.DEPRESSED], BlockType.CLEAR);
			Rough = Parse.DefaultIfNull(attributes[(int)Attribute.ROUGH], BlockType.CLEAR);
			Sloped = Parse.DefaultIfNull(attributes[(int)Attribute.SLOPED], BlockType.CLEAR);
			Swamp = Parse.DefaultIfNull(attributes[(int)Attribute.SWAMP], BlockType.CLEAR);
			Water = Parse.DefaultIfNull(attributes[(int)Attribute.WATER], BlockType.IMPASSABLE);

			IgnoresEnvironmentMovement = Parse.DefaultIfNull(
				attributes[(int)Attribute.IGNORES_ENVIRONMENT_MOVEMENT], false);
			CannotUseRoadMovement = Parse.DefaultIfNull(
				attributes[(int)Attribute.CANNOT_USE_ROAD_MOVEMENT], false);
		}
	}
}
