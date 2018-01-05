using System;
using System.Diagnostics;
using System.IO;

using Cardamom.Serialization;

using SFML.Graphics;

namespace PanzerBlitz
{
	public static class FileUtils
	{
		public static FileStream GetStream(string Path, FileMode FileMode, int Timeout)
		{
			var time = Stopwatch.StartNew();
			while (time.ElapsedMilliseconds < Timeout)
			{
				try
				{
					return new FileStream(Path, FileMode);
				}
				catch (IOException e)
				{
					if (e.HResult != -2147024864)
						throw;
				}
			}

			throw new TimeoutException($"Failed to get a handle to {Path} within {Timeout}ms.");
		}

		public static void SerializeColor(SerializationOutputStream Stream, Color Color)
		{
			Stream.Write(Color.R);
			Stream.Write(Color.G);
			Stream.Write(Color.B);
			Stream.Write(Color.A);
		}

		public static Color DeserializeColor(SerializationInputStream Stream)
		{
			return new Color(Stream.ReadByte(), Stream.ReadByte(), Stream.ReadByte(), Stream.ReadByte());
		}
	}
}
