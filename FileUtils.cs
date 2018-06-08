using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Cardamom.Serialization;
using Cardamom.Utilities.Markov;

using SFML.Graphics;

namespace PanzerBlitz
{
	public static class FileUtils
	{
		public static T Print<T>(T Object)
		{
			Console.WriteLine(Object);
			return Object;
		}

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

		public static MarkovGenerator<char> LoadLanguage(string Path)
		{
			using (FileStream fileStream = new FileStream(Path, FileMode.Open))
			{
				using (GZipStream compressionStream = new GZipStream(fileStream, CompressionMode.Decompress))
				{
					var stream = new SerializationInputStream(compressionStream);
					return new MarkovGenerator<char>(stream);
				}
			}
		}

		public static MarkovGenerator<char> GenerateLanguage(string ExamplePath)
		{
			var g = new MarkovGenerator<char>(3);
			var parentheticals = new Regex("\\(.*\\)");
			var capitals = new Regex("[A-Z- ]*");
			foreach (var line in File.ReadAllLines(ExamplePath, Encoding.UTF8))
			{
				var name = line.ToLower();
				if (name.Length == 0) continue;
				name = parentheticals.Replace(name, "");
				name = capitals.Replace(name, "");
				name = name.Trim();
				g.AddSequence(name);
			}
			return g;
		}

		public static void MungeLanguage(string ExamplePath, string OutputPath)
		{
			var g = GenerateLanguage(ExamplePath);
			using (FileStream fileStream = new FileStream(OutputPath, FileMode.Create))
			{
				using (GZipStream compressionStream = new GZipStream(fileStream, CompressionLevel.Optimal))
				{
					var stream = new SerializationOutputStream(compressionStream);
					stream.Write(g);
				}
			}
		}

		public static void Remap()
		{
			foreach (string file in Directory.EnumerateFiles("./Maps/"))
			{
				Map map;
				using (FileStream stream = new FileStream(file, FileMode.Open))
				{
					using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Decompress))
					{
						map = new Map(new SerializationInputStream(compressionStream), null, new IdGenerator());
					}
				}
				using (FileStream stream = new FileStream(file, FileMode.OpenOrCreate))
				{
					using (GZipStream compressionStream = new GZipStream(stream, CompressionLevel.Optimal))
					{
						map.Serialize(new SerializationOutputStream(compressionStream));
					}
				}
			}
		}
	}
}
