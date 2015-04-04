using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatchImageProcessor.ViewModel;

namespace BatchImageProcessor.Model
{
	public static class Engine
	{
		private static ViewModel.ViewModel _model;
		public static int TotalImages;
		public static int DoneImages;
		public static event EventHandler UpdateDone;
		public static bool Cancel = false;
		private static readonly Mutex NamingMutex = new Mutex();

		public static void Process(ViewModel.ViewModel vm)
		{
			_model = vm;
			TotalImages = 0;
			DoneImages = 0;
			int m, n;
			ThreadPool.GetMaxThreads(out m, out n);
			Trace.WriteLine(string.Format("Max threads: {0}", m));
			Task.Factory.StartNew(() => QueueItems(vm.Folders[0], vm.OutputPath));
		}

		private static void QueueItems(Folder folder, string path)
		{
			var enumerable = folder.Files.OfType<FileWrapper>().Where(wrapper => wrapper.Selected).Cast<File>();
			var fileWrappers = enumerable as List<File> ?? enumerable.ToList();
			fileWrappers.ForEach(o =>
			{
				o.OutputPath = path;
				o.ImageNumber = TotalImages++;
			});

			Parallel.ForEach(fileWrappers, ProcessImage);

			var enumerable1 = folder.Files.OfType<Folder>();
			var list = enumerable1 as List<Folder> ?? enumerable1.ToList();
			list.ForEach(fold => QueueItems(fold, Path.Combine(path, fold.Name)));
		}

		public static void ProcessImage(File w)
		{
			if (Cancel)
			{
				Interlocked.Decrement(ref TotalImages);
				UpdateDone?.Invoke(null, EventArgs.Empty);
				return;
			}

			if (w != null)
			{
				var outFmt = w.OverrideFormat == Format.Default ? _model.OutputFormat : w.OverrideFormat;

				// Load Image
				Image b = null;
				try
				{
					b = StaticImageUtils.LoadImage(w.Path);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine(e);
				}

				if (b == null)
				{
					Interlocked.Decrement(ref TotalImages);
					UpdateDone?.Invoke(null, EventArgs.Empty);
					return;
				}

				#region Process Steps

				if (w.OverrideRotation != Rotation.Default || _model.EnableRotation)
					b.RotateImage(w.OverrideRotation == Rotation.Default ? _model.DefaultRotation : w.OverrideRotation);

				if (_model.EnableResize && !w.OverrideResize)
					b = b.ResizeImage(new Size(_model.ResizeWidth, _model.ResizeHeight), _model.DefaultResizeMode, _model.UseAspectRatio);

				if (_model.EnableCrop && !w.OverrideCrop)
					b = b.CropImage(new Size(_model.CropWidth, _model.CropHeight), _model.DefaultCropAlignment);

				if (_model.EnableWatermark && !w.OverrideWatermark)
					b.WatermarkImage(_model.WatermarkAlignment, (float)_model.WatermarkOpacity, _model.DefaultWatermarkType, _model.WatermarkText, _model.WatermarkFont, _model.WatermarkImagePath, _model.WatermarkGreyscale);

				if (_model.EnableColor && !w.OverrideColor)
					b = b.ColorImage((float)_model.ColorSaturation, _model.ColorType, _model.ColorContrast, _model.ColorBrightness, (float)_model.ColorGamma);

				#endregion

				// Filename
				var name = GenerateFilename(w, b);

				// Output Path
				var outpath = GenerateOutputPath(w, name, outFmt);

				#region Select Encoder

				var encoder = ImageFormat.Jpeg;
				// Save
				switch (outFmt)
				{
					case Format.Png:
						encoder = ImageFormat.Png;
						break;
					case Format.Gif:
						encoder = ImageFormat.Gif;
						break;
					case Format.Tiff:
						encoder = ImageFormat.Tiff;
						break;
					case Format.Bmp:
						encoder = ImageFormat.Bmp;
						break;
				}

				#endregion

				if (outFmt == Format.Jpg)
				{
					var myEncoderParameters = new EncoderParameters(1);
					var myencoder = Encoder.Quality;
					var myEncoderParameter = new EncoderParameter(myencoder, (long)(_model.JpegQuality * 100));
					myEncoderParameters.Param[0] = myEncoderParameter;					
					b.Save(outpath, StaticImageUtils.GetEncoder(encoder), myEncoderParameters);
				}
				else
					b.Save(outpath, encoder);
				b.Dispose();
				outpath.Close();
			}

			Interlocked.Increment(ref DoneImages);
			UpdateDone?.Invoke(null, EventArgs.Empty);
		}

		private static FileStream GenerateOutputPath(File w, string name, Format outputFormat)
		{
			var ext = ".jpg";
			switch (outputFormat)
			{
				case Format.Png:
					ext = ".png";
					break;
				case Format.Gif:
					ext = ".gif";
					break;
				case Format.Tiff:
					ext = ".tiff";
					break;
				case Format.Bmp:
					ext = ".bmp";
					break;
			}

			var outpath = Path.Combine(w.OutputPath, name + ext);
			if (!Directory.Exists(w.OutputPath))
				Directory.CreateDirectory(w.OutputPath);

			var outpathFormat = outpath.Replace(ext, " ({0})" + ext);

			NamingMutex.WaitOne();
			if (System.IO.File.Exists(outpath))
			{
				var i = 0;
				while (System.IO.File.Exists(string.Format(outpathFormat, ++i))) ;
				outpath = string.Format(outpathFormat, i);
			}

			var ret = System.IO.File.Create(outpath);
			NamingMutex.ReleaseMutex();
			return ret;
		}

		private static string GenerateFilename(File w, Image b)
		{
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
			return name;
		}
	}
}