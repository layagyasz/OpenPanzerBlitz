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
	}
}
