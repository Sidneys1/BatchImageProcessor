using System;
using System.Drawing;
using System.IO;
using System.Runtime.Caching;
using System.Threading;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Shell;

namespace BatchImageProcessor.Model
{
	public class WeakThumbnail
	{
		private static readonly ObjectCache Cache = MemoryCache.Default;

		private bool _set;
		//WeakReference<ImageSource> weak = new WeakReference<ImageSource>(null);

		public WeakThumbnail(string path)
		{
			Path = path;
			//weak = new WeakReference<ImageSource>(null, true);
			//gmaSource = new GenMeASource(GenMeSource);
		}

		public string Path { get; set; }

		public ImageSource Source
		{
			set
			{
				_set = true;
				//weak.SetTarget(value);
				Cache.Add(Path, value, new DateTimeOffset(DateTime.UtcNow + new TimeSpan(0, 3, 0)));
				SourceUpdated();
			}
			get
			{
				//ImageSource attempt = null;
				//if (set && weak.TryGetTarget(out attempt))
				//	if (attempt != null)
				//		return attempt;
				//	else
				//		set = false;

				if (_set && Cache.Contains(Path))
					return Cache.Get(Path) as ImageSource;

				_set = false;
				//GenSource();
				ThreadPool.QueueUserWorkItem(GenSource);

				//throw new Exception();
				return null;
			}
		}

		private static Bitmap GetThumb(string path)
		{
			try
			{
				if (System.IO.File.Exists(path))
				{
					if (!ShellObject.IsPlatformSupported)
						throw new NotSupportedException("Platform does not support ShellFiles.");

					var sFile = ShellFile.FromFilePath(path);
					return sFile.Thumbnail.LargeBitmap;
				}
				if (!Directory.Exists(path))
					throw new FileNotFoundException(string.Format(@"File at ""{0}"" does not exist.", path));
				if (!ShellObject.IsPlatformSupported)
					throw new NotSupportedException("Platform does not support ShellFolders.");

				var shellFile = ShellObject.FromParsingName(path);
				return shellFile.Thumbnail.Bitmap;
			}
			catch
			{
				return null;
			}
		}

		public void GenSource(object o = null)
		{
			using (var b = GetThumb(Path))
			{
				ImageSource ret = b.GetSource();
				ret.Freeze();
				Source = ret;
			}
		}

		public event Action SourceUpdated = delegate { };
	}
}