using System;
using System.IO;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;

namespace BatchImageProcessor.ViewModel
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
				Task.Factory.StartNew(() => GenSource());
				return null;
			}
		}

		private static BitmapSource GetThumb(string path)
		{
			try
			{
				if (!File.Exists(path) && !Directory.Exists(path))
					throw new FileNotFoundException($@"File at ""{path}"" does not exist.");
				if (!ShellObject.IsPlatformSupported)
					throw new NotSupportedException("Platform does not support ShellObjects.");

				var sFile = ShellFile.FromFilePath(path);
				var bitmapSource = sFile.Thumbnail.BitmapSource;
				sFile.Dispose();
				return bitmapSource;
			}
			catch(Exception)
			{
				return null;
			}
		}

		public void GenSource(object o = null)
		{
			var ret = GetThumb(Path);

			if (ret == null)
				return;

			ret.Freeze();
			Source = ret;
		}

		public event Action SourceUpdated = delegate { };
	}
}