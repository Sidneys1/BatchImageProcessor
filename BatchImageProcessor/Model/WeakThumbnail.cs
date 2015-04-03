using System;
using System.IO;
using System.Runtime.Caching;
using System.Threading;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;

namespace BatchImageProcessor.Model
{
	public class WeakThumbnail
	{
		private static readonly ObjectCache Cache = MemoryCache.Default;
		private bool _set;

		public WeakThumbnail(string path)
		{
			Path = path;
		}

		public string Path { get; set; }

		public BitmapSource Source
		{
			set
			{
				_set = true;
				Cache.Add(Path, value, new DateTimeOffset(DateTime.UtcNow + new TimeSpan(0, 3, 0)));
				SourceUpdated();
			}
			get
			{
				if (_set && Cache.Contains(Path))
					return Cache.Get(Path) as BitmapSource;

				_set = false;
				ThreadPool.QueueUserWorkItem(GenSource);
				return null;
			}
		}

		private static BitmapSource GetThumb(string path)
		{
			try
			{
				if (!System.IO.File.Exists(path) && !Directory.Exists(path))
					throw new FileNotFoundException(string.Format(@"File at ""{0}"" does not exist.", path));
				if (!ShellObject.IsPlatformSupported)
					throw new NotSupportedException("Platform does not support ShellObjects.");

				var sFile = ShellObject.FromParsingName(path);
				return sFile.Thumbnail.BitmapSource;
			}
			catch
			{
				return null;
			}
		}

		public void GenSource(object o = null)
		{
			var ret = GetThumb(Path);
			ret.Freeze();
			Source = ret;
		}

		public event Action SourceUpdated = delegate { };
	}
}