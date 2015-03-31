using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using BatchImageProcessor.ViewModel;

namespace BatchImageProcessor.Model
{
	public static class Engine //(ViewModel.ViewModel model)
	{
		private static ViewModel.ViewModel _model; //= model;
		public static int TotalImages;
		public static int DoneImages;
		public static event EventHandler UpdateDone;
	    public static bool Cancel=false;

		public static void Process(ViewModel.ViewModel vm)
		{
			_model = vm;
			TotalImages = 0;
			DoneImages = 0;
		    int m,n;
            ThreadPool.GetMaxThreads(out m, out n);
            Trace.WriteLine(string.Format("Max threads: {0}", m));
			QueueItems(vm.Folders[0], vm.OutputPath);
		}

		private static void QueueItems(Folder folder, string path)
		{
			foreach (var wrapper in folder.Files.OfType<FileWrapper>().Where(wrapper => wrapper.Selected))
			{
				wrapper.OutputPath = path;
				wrapper.ImageNumber = TotalImages;
				if (!ThreadPool.QueueUserWorkItem(ProcessImage, wrapper))
					Debug.WriteLine("Could not queue #{0}: {1}", TotalImages, wrapper.Name);
				else
					TotalImages++;
			}

			foreach (var fold in folder.Files.OfType<Folder>())
			{
				QueueItems(fold, Path.Combine(path, fold.Name));
			}
		}

		public static void ProcessImage(object o)
		{
		    if (Cancel)
		    {
		        Interlocked.Decrement(ref TotalImages);
                UpdateDone?.Invoke(null, EventArgs.Empty);
		        return;
		    }

		    var w = (o as FileWrapper);

			if (w != null)
			{
				var b = Image.FromFile(w.Path);
				// Process
				if (w.RotationOverride != Rotation.Default || _model.EnableRotation)
					RotateImage(w, b);

				if (_model.EnableResize && !w.OverrideResize)
					b = ResizeImage(b);

				if (_model.EnableCrop && !w.OverrideCrop)
					b = CropImage(b);

				// TODO: Watermark
				//if (_model.EnableWatermark && !w.OverrideWatermark)
				//WatermarkImage(w, b);

				// Filename
				string name = null;
				switch (_model.NameOption)
				{
					case NameType.Original:
						name = w.Name;
						break;
					case NameType.Numbered:
						name = w.ImageNumber.ToString(CultureInfo.InvariantCulture);
						break;
					case NameType.Custom:
						name = _model.OutputTemplate;
						name = name.Replace("{o}", w.Name);
						name = name.Replace("{w}", b.Width.ToString(CultureInfo.InvariantCulture));
						name = name.Replace("{h}", b.Height.ToString(CultureInfo.InvariantCulture));
						break;
				}

				// output path
				var outpath = Path.Combine(w.OutputPath, name + ".jpg");
				if (!Directory.Exists(w.OutputPath))
					Directory.CreateDirectory(w.OutputPath);

				var outpathFormat = outpath.Replace(".jpg", " ({0}).jpg");

				if (System.IO.File.Exists(outpath))
				{
					var i = 0;
				    while (System.IO.File.Exists(string.Format(outpathFormat, ++i))) ;
					outpath = string.Format(outpathFormat, i);
				}

				// Save
				b.Save(outpath);
				b.Dispose();
			}

			Interlocked.Increment(ref DoneImages);
			//if (UpdateDone != null)
			UpdateDone?.Invoke(null, EventArgs.Empty);
		}

		// TODO: Watermark

		//private static void WatermarkImage(FileWrapper w, Image b)
		//{
		//	if (_model.DefaultWatermarkType == WatermarkType.Text)
		//	{

		//	}
		//	else
		//	{

		//	}
		//}

		private static Image CropImage(Image b)
		{
			var cropSize = new Size(_model.CropWidth, _model.CropHeight);

			if (cropSize.Width > b.Width || cropSize.Height > b.Height)
				cropSize = new Size(
					cropSize.Width > b.Width ? b.Width : cropSize.Width,
					cropSize.Height > b.Height ? b.Height : cropSize.Height);

			int x = 0, y = 0;

			switch (_model.DefaultCropAlignment)
			{
				case Alignment.Middle_Left:
				case Alignment.Middle_Center:
				case Alignment.Middle_Right:
					y = ((b.Height/2) - (cropSize.Height/2));
					break;

				case Alignment.Bottom_Left:
				case Alignment.Bottom_Center:
				case Alignment.Bottom_Right:
					y = (b.Height - cropSize.Height);
					break;
			}

			switch (_model.DefaultCropAlignment)
			{
				case Alignment.Top_Center:
				case Alignment.Middle_Center:
				case Alignment.Bottom_Center:
					x = ((b.Width/2) - (cropSize.Width/2));
					break;

				case Alignment.Top_Right:
				case Alignment.Middle_Right:
				case Alignment.Bottom_Right:
					x = b.Width - cropSize.Width;
					break;
			}

			var r = new Rectangle(new Point(x, y), cropSize);

			Image ret = new Bitmap(cropSize.Width, cropSize.Height);

			using (var g = Graphics.FromImage(ret))
				g.DrawImage(b, new Rectangle(Point.Empty, cropSize), r, GraphicsUnit.Pixel);

			b.Dispose();
			return ret;
		}

		private static Image ResizeImage(Image b)
		{
			var newSize = b.Size;
			var targetSize = new Size(_model.ResizeWidth, _model.ResizeHeight);

			if (_model.UseAspectRatio)
			{
				if (b.Width > b.Height) // landscape
				{
					targetSize = new Size(
						targetSize.Width > targetSize.Height ? targetSize.Width : targetSize.Height,
						targetSize.Width > targetSize.Height ? targetSize.Height : targetSize.Width);
				}
				else if (b.Height > b.Width) // Portrait
				{
					targetSize = new Size(
						targetSize.Height > targetSize.Width ? targetSize.Width : targetSize.Height,
						targetSize.Height > targetSize.Width ? targetSize.Height : targetSize.Width);
				}
			}

			switch (_model.DefaultResizeMode)
			{
				case ResizeMode.Smaller:
					if (b.Width > targetSize.Width || b.Height > targetSize.Height)
					{
						var ratioX = targetSize.Width/(double) b.Width;
						var ratioY = targetSize.Height/(double) b.Height;
						// use whichever multiplier is smaller
						var ratio = ratioX < ratioY ? ratioX : ratioY;

						// now we can get the new height and width
						var newHeight = Convert.ToInt32(b.Height*ratio);
						var newWidth = Convert.ToInt32(b.Width*ratio);

						newSize = new Size(newWidth, newHeight);
					}
					break;
				case ResizeMode.Larger:
					if (b.Width < targetSize.Width || b.Height < targetSize.Height)
					{
						var ratioX = targetSize.Width/(double) b.Width;
						var ratioY = targetSize.Height/(double) b.Height;
						// use whichever multiplier is larger
						var ratio = ratioX > ratioY ? ratioX : ratioY;

						// now we can get the new height and width
						var newHeight = Convert.ToInt32(b.Height*ratio);
						var newWidth = Convert.ToInt32(b.Width*ratio);

						newSize = new Size(newWidth, newHeight);
					}
					break;
				case ResizeMode.Exact:
					if (b.Size != targetSize)
					{
						newSize = new Size(targetSize.Width, targetSize.Height);
					}
					break;
			}

			if (b.Size == newSize) return b;
			var one = b;
			Image two = new Bitmap(b, newSize);
			one.Dispose();
			return two;
		}

		private static void RotateImage(FileWrapper w, Image b)
		{
			var r = w.RotationOverride == Rotation.Default ? _model.DefaultRotation : w.RotationOverride;

			switch (r)
			{
				case Rotation.Clockwise:
					b.RotateFlip(RotateFlipType.Rotate90FlipNone);
					break;
				case Rotation.CounterClockwise:
					b.RotateFlip(RotateFlipType.Rotate270FlipNone);
					break;
				case Rotation.UpsideDown:
					b.RotateFlip(RotateFlipType.Rotate180FlipNone);
					break;
				case Rotation.Portrait:
					if (b.Width > b.Height)
						b.RotateFlip(RotateFlipType.Rotate270FlipNone);
					break;
				case Rotation.Landscape:
					if (b.Width < b.Height)
						b.RotateFlip(RotateFlipType.Rotate90FlipNone);
					break;
			}
		}
	}
}