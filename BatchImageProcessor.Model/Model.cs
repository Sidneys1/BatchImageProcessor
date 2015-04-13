using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatchImageProcessor.Model.Interface;
using BatchImageProcessor.Model.Types;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model
{
	public class Model
	{
		private int _totalImages;
		private int _doneImages;


		public bool Cancel = false;


		public bool OutputSet { get; set; } = false;

		public ObservableCollection<IFolderableHost> Folders { get; } = new ObservableCollection<IFolderableHost>();

		private readonly bool _console;
		public OptionSet Options { get; } = new OptionSet();

		public Model() { }

		public Model(OptionSet x, List<string> files)
		{
			var rootFolder = new Folder();
			Folders.Add(rootFolder);

			Options = x;
			_console = true;

			files.ForEach(o => rootFolder.Files.Add(new File(o)));
		}

		public async Task Process(IProgress<ModelProgressUpdate> progress = null)
		{
			_totalImages = 0;
			_doneImages = 0;

			if (_console)
				QueueItems(Folders[0], Options.OutputOptions.OutputPath, progress);
			else
				await Task.Factory.StartNew(() => QueueItems(Folders[0], Options.OutputOptions.OutputPath, progress));
		}

		private void QueueItems(IFolderableHost folderWrapper, string path, IProgress<ModelProgressUpdate> progress = null)
		{
			var enumerable = folderWrapper.Files.OfType<IFile>().Where(wrapper => wrapper.Selected);
			var fileWrappers = enumerable as List<IFile> ?? enumerable.ToList();
			fileWrappers.ForEach(o =>
			{
				o.OutputPath = path;
				o.ImageNumber = _totalImages++;
			});

			progress?.Report(new ModelProgressUpdate(_totalImages, _doneImages));

			Parallel.ForEach(fileWrappers, o => ProcessImage(o, progress));

			var enumerable1 = folderWrapper.Files.OfType<IFolderableHost>();
			var list = enumerable1 as List<IFolderableHost> ?? enumerable1.ToList();
			list.ForEach(fold => QueueItems(fold, Path.Combine(path, fold.Name)));
		}

		private void ProcessImage(IFile w, IProgress<ModelProgressUpdate> progress = null)
		{
			if (Cancel)
			{
				Interlocked.Decrement(ref _totalImages);
				progress?.Report(new ModelProgressUpdate(_totalImages, _doneImages));
				return;
			}

			if (w != null)
			{
				var outFmt = w.Options.OutputOptions.OutputFormat == Format.Default ? Options.OutputOptions.OutputFormat : w.Options.OutputOptions.OutputFormat;

				// Load Image
				Image b = null;
				try
				{
					b = StaticImageUtils.LoadImage(w);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine(e);
				}

				if (b == null)
				{
					Interlocked.Decrement(ref _totalImages);
					progress?.Report(new ModelProgressUpdate(_totalImages, _doneImages));
					return;
				}

				#region Process Steps

				var clone = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppPArgb);
				using (var gr = Graphics.FromImage(clone))
				{
					gr.DrawImage(b, new Rectangle(0, 0, clone.Width, clone.Height));
				}
				b.Dispose();
				b = clone;

				if (w.Options.Rotation != Rotation.Default || Options.EnableRotation)
					b.RotateImage(w.Options.EnableRotation ? w.Options.Rotation : Options.Rotation);

				if (Options.EnableResize || w.Options.EnableResize)
					b = b.ResizeImage(w.Options.EnableResize ? w.Options.ResizeOptions : Options.ResizeOptions);

				if (Options.EnableCrop || w.Options.EnableCrop)
					b = b.CropImage(w.Options.EnableCrop ? w.Options.CropOptions : Options.CropOptions);

				if (Options.EnableWatermark || w.Options.EnableWatermark)
					b.WatermarkImage(w.Options.EnableWatermark ? w.Options.WatermarkOptions : Options.WatermarkOptions);

				b = b.AdjustImage(w.Options.EnableAdjustments ? w.Options.AdjustmentOptions : Options.AdjustmentOptions);

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
					var myEncoderParameter = new EncoderParameter(myencoder, (long)(Options.OutputOptions.JpegQuality * 100));
					myEncoderParameters.Param[0] = myEncoderParameter;
					b.Save(outpath, StaticImageUtils.GetEncoder(encoder), myEncoderParameters);
				}
				else
					b.Save(outpath, encoder);
				b.Dispose();
				outpath.Close();
			}

			Interlocked.Increment(ref _doneImages);
			progress?.Report(new ModelProgressUpdate(_totalImages, _doneImages));
		}

		private FileStream GenerateOutputPath(IFile w, string name, Format outputFormat)
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

			FileStream ret;
			lock (this)
			{
				if (System.IO.File.Exists(outpath))
				{
					var i = 0;
					while (System.IO.File.Exists(string.Format(outpathFormat, ++i)))
					{
					}
					outpath = string.Format(outpathFormat, i);
				}

				ret = System.IO.File.Create(outpath);
			}
			return ret;
		}

		private string GenerateFilename(IFile w, Image b)
		{
			string name = null;
			switch (Options.OutputOptions.NameOption)
			{
				case NameType.Original:
					name = w.Name;
					break;
				case NameType.Numbered:
					name = w.ImageNumber.ToString(CultureInfo.InvariantCulture);
					break;
				case NameType.Custom:
					name = Options.OutputOptions.OutputTemplate;
					name = name.Replace("{o}", w.Name);
					name = name.Replace("{w}", b.Width.ToString(CultureInfo.InvariantCulture));
					name = name.Replace("{h}", b.Height.ToString(CultureInfo.InvariantCulture));
					break;
			}
			return name;
		}
	}
}