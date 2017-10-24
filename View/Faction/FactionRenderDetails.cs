using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class FactionRenderDetails
	{
		enum Attribute { IMAGE_PATH }

		public readonly string ImagePath;

		public FactionRenderDetails(ParseBlock Block, string Path)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			ImagePath = Path + (string)attributes[(int)Attribute.IMAGE_PATH];
		}
	}
}
