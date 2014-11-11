using System.Runtime.Caching;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Media;

namespace BatchImageProcessor.Model
{
	public class WeakThumbnail
	{
		static readonly ObjectCache Cache = MemoryCache.Default;

		public string Path { get; set; }

		bool _set;
		//WeakReference<ImageSource> weak = new WeakReference<ImageSource>(null);

		public ImageSource Source
		{

			set
			{
				_set = true;
				//weak.SetTarget(value);
				Cache.Add(Path, value, new DateTimeOffset(DateTime.UtcNow + new TimeSpan(0, 0, 10)));
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

		public WeakThumbnail(string path)
		{
			Path = path;
			//weak = new WeakReference<ImageSource>(null, true);
			//gmaSource = new GenMeASource(GenMeSource);
		}

		static Bitmap GetThumb(string path)
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
