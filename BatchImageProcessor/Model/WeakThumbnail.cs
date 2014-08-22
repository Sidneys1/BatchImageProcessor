using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BatchImageProcessor.Model
{
	public class WeakThumbnail: INotifyPropertyChanged
	{
		//private delegate void GenMeASource();
		//private GenMeASource gmaSource;
		public string Path { get; set; }

		bool set = false;
		WeakReference<ImageSource> weak;
		//ImageSource _source = null;
		public ImageSource Source
		{
			get
			{
				//return _source ?? (_source = GenSource());

				ImageSource attempt = null;
				if (set && weak.TryGetTarget(out attempt))
					if (attempt != null)
						return attempt;
					else
						set = false;

				set = true;
				attempt = GenSource();
				//gmaSource.BeginInvoke(null, null);
				weak.SetTarget(attempt);
				return attempt;
			}
		}

		public WeakThumbnail(string path)
		{
			Path = path;
			weak = new WeakReference<ImageSource>(null, true);
			//gmaSource = new GenMeASource(GenMeSource);
		}

		Bitmap GetThumb(string Path)
		{
			if (System.IO.File.Exists(Path))
			{
				if (!ShellFile.IsPlatformSupported)
					throw new NotSupportedException("Platform does not support ShellFiles.");

				ShellFile sFile = ShellFile.FromFilePath(Path);
				return sFile.Thumbnail.LargeBitmap;
			}
			else if (System.IO.Directory.Exists(Path))
			{
				if (!ShellFolder.IsPlatformSupported)
					throw new NotSupportedException("Platform does not support ShellFolders.");

				ShellObject sFile = ShellFolder.FromParsingName(Path);
				return sFile.Thumbnail.Bitmap;
			}
			else
				throw new FileNotFoundException(string.Format(@"File at ""{0}"" does not exist.", Path));
		} 

		public ImageSource GenSource()
		{
			ImageSource ret;
			using (Bitmap b = GetThumb(Path))
			{
				ret = b.GetSource();
				ret.Freeze();
				return ret;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged = delegate { };
	}
}
