// Copyright (c) 2017 Sidneys1
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
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