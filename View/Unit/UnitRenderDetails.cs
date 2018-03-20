using Cardamom.Serialization;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class UnitRenderDetails : Serializable
	{
		enum Attribute { OVERRIDE_DISPLAY_NAME, OVERRIDE_COLOR, IMAGE_PATH }

		public readonly string OverrideDisplayName;
		public readonly Color OverrideColor;

		readonly string _RootPath;
		readonly string _ImagePath;

		public string ImagePath
		{
			get
			{
				return _RootPath + _ImagePath;
			}
		}

		public UnitRenderDetails(SerializationInputStream Stream, string Path)
		{
			_RootPath = Path;

			if (Stream.ReadBoolean()) OverrideDisplayName = Stream.ReadString();
			OverrideColor = FileUtils.DeserializeColor(Stream);
			_ImagePath = Stream.ReadString();
		}

		public UnitRenderDetails(ParseBlock Block, string Path)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			OverrideDisplayName = (string)attributes[(int)Attribute.OVERRIDE_DISPLAY_NAME];
			OverrideColor = (Color)(attributes[(int)Attribute.OVERRIDE_COLOR] ?? Color.Black);
			_RootPath = Path;
			_ImagePath = (string)attributes[(int)Attribute.IMAGE_PATH];
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(OverrideDisplayName != null);
			if (OverrideDisplayName != null) Stream.Write(OverrideDisplayName);
			FileUtils.SerializeColor(Stream, OverrideColor);
			Stream.Write(_ImagePath);
		}
	}
}
