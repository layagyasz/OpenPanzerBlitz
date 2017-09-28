using System;

using Cardamom.Serialization;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class UnitRenderDetails
	{
		enum Attribute { OVERRIDE_DISPLAY_NAME, OVERRIDE_COLOR, IMAGE_PATH }

		public readonly string OverrideDisplayName;
		public readonly Color OverrideColor;
		public readonly string ImagePath;

		public UnitRenderDetails(ParseBlock Block, string Path)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			OverrideDisplayName = (string)attributes[(int)Attribute.OVERRIDE_DISPLAY_NAME];
			OverrideColor = Parse.DefaultIfNull(attributes[(int)Attribute.OVERRIDE_COLOR], Color.Black);
			ImagePath = Path + (string)attributes[(int)Attribute.IMAGE_PATH];
		}
	}
}
