using System;
using System.Diagnostics;
using System.IO;

namespace PanzerBlitz
{
	public static class FileUtils
	{
		public static FileStream GetStream(string Path, FileMode FileMode, int Timeout)
		{
			var time = Stopwatch.StartNew();
			while (time.ElapsedMilliseconds < Timeout)
				return new FileStream(Path, FileMode);

			throw new TimeoutException($"Failed to get a write handle to {Path} within {Timeout}ms.");
		}
	}
}
