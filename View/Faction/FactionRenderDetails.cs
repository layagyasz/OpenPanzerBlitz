using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class FactionRenderDetails : Serializable
	{
		enum Attribute { IMAGE_PATH }

		readonly string _RootPath;
		readonly string _ImagePath;

		public string ImagePath
		{
			get
			{
				return _RootPath + _ImagePath;
			}
		}

		public FactionRenderDetails(ParseBlock Block, string Path)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_RootPath = Path;
			_ImagePath = (string)attributes[(int)Attribute.IMAGE_PATH];
		}

		public FactionRenderDetails(SerializationInputStream Stream, string Path)
		{
			_RootPath = Path;
			_ImagePath = Stream.ReadString();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_ImagePath);
		}
	}
}
