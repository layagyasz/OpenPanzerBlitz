using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class FactionRenderDetails : Serializable
	{
		enum Attribute { IMAGE_PATH }

		string _RootPath;
		string _ImagePath;

		public string ImagePath
		{
			get
			{
				return _RootPath + _ImagePath;
			}
		}

		public FactionRenderDetails(ParseBlock Block, string Path)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
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
