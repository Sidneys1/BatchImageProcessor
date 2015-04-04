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
using BatchImageProcessor.Interface;
using BatchImageProcessor.Types;

namespace BatchImageProcessor.Model
{
	public class Model
	{
		public int TotalImages;
		public int DoneImages;
		public event EventHandler UpdateDone;
		public bool Cancel = false;
		private readonly Mutex _namingMutex = new Mutex();
		public string OutputPath;
		public NameType NameOption = NameType.Original;
		public Format OutputFormat = Format.Jpg;
		public double JpegQuality = 0.95;
		public bool OutputSet = false;
		public string OutputTemplate;
		public bool EnableCrop;
		public bool EnableResize = false;
		public bool EnableRotation = false;
		public bool EnableWatermark;
		public Rotation DefaultRotation = Rotation.None;
		public int CropHeight = 600;
		public int CropWidth = 800;
		public Alignment DefaultCropAlignment = Alignment.Middle_Center;
		public ResizeMode DefaultResizeMode = ResizeMode.Smaller;
		public int ResizeHeight = 600;
		public int ResizeWidth = 800;
		public bool UseAspectRatio = true;
		public WatermarkType DefaultWatermarkType = WatermarkType.Text;
		public Alignment WatermarkAlignment = Alignment.Bottom_Right;
		public Font WatermarkFont = new Font("Calibri", 12f);
		public bool WatermarkGreyscale = true;
		public string WatermarkImagePath;
		public double WatermarkOpacity = 0.7;
		public string WatermarkText;
		public ColorType ColorType = ColorType.Saturation;
		public double ColorBrightness = 1.0;
		public double ColorContrast = 1.0;
		public double ColorSaturation = 1.0;
		public double ColorGamma = 1.0;
		public ObservableCollection<IFolderableHost> Folders { get; } = new ObservableCollection<IFolderableHost>();
		private bool console = false;

		public Model() { }

		public Model(OptionStruct x)
		{
			var rootFolder = new Folder();
			Folders.Add(rootFolder);

			EnableCrop = x.Crop;
			EnableWatermark = x.Watermark;
			UseAspectRatio = x.SizeSmart;
			WatermarkGreyscale = x.WatermarkGrey;
			OutputPath = x.Output;
			Enum.TryParse(x.Format, out OutputFormat);
			Enum.TryParse(x.WatermarkType, out DefaultWatermarkType);
			WatermarkText = WatermarkImagePath = x.WatermarkText;
			WatermarkFont = new Font(x.WatermarkFont, x.WatermarkFontsize, GraphicsUnit.Point);
			DefaultResizeMode = (ResizeMode)x.Size;
			EnableResize = DefaultResizeMode != ResizeMode.None;
			DefaultRotation = (Rotation)x.Rotation;
			EnableRotation = DefaultRotation != Rotation.None;
			ResizeWidth = x.SizeWidth;
			ResizeHeight = x.SizeHeight;
			CropWidth = x.CropWidth;
			CropHeight = x.CropHeight;
			DefaultCropAlignment = (Alignment)x.CropAlign;
			WatermarkAlignment = (Alignment)x.WatermarkAlign;
			ColorType = (ColorType)x.ColorSatMode;
			WatermarkOpacity = x.WatermarkOpac;
			ColorBrightness = x.ColorBright;
			ColorContrast = x.ColorContrast;
			ColorGamma = x.ColorGamma;
			ColorSaturation = x.ColorSat;
			JpegQuality = x.OutJpeg;
			console = true;
			x.Files.ForEach(o => rootFolder.Files.Add(new File(o)));
		}

		public void Process()
		{
			TotalImages = 0;
			DoneImages = 0;
			if (console)
				QueueItems(Folders[0], OutputPath);
			else
				Task.Factory.StartNew(() => QueueItems(Folders[0], OutputPath));

		}

		private void QueueItems(IFolderableHost folderWrapper, string path)
		{
			var enumerable = folderWrapper.Files.OfType<IFile>().Where(wrapper => wrapper.Selected);
			var fileWrappers = enumerable as List<IFile> ?? enumerable.ToList();
			fileWrappers.ForEach(o =>
			{
				o.OutputPath = path;
				o.ImageNumber = TotalImages++;
			});

			Parallel.ForEach(fileWrappers, ProcessImage);

			var enumerable1 = folderWrapper.Files.OfType<IFolderableHost>();
			var list = enumerable1 as List<IFolderableHost> ?? enumerable1.ToList();
			list.ForEach(fold => QueueItems(fold, Path.Combine(path, fold.Name)));
		}

		public void ProcessImage(IFile w)
		{
			if (Cancel)
			{
				Interlocked.Decrement(ref TotalImages);
				UpdateDone?.Invoke(null, EventArgs.Empty);
				return;
			}

			if (w != null)
			{
				var outFmt = w.OverrideFormat == Format.Default ? OutputFormat : w.OverrideFormat;

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

				if (w.OverrideRotation != Rotation.Default || EnableRotation)
					b.RotateImage(w.OverrideRotation == Rotation.Default ? DefaultRotation : w.OverrideRotation);

				if (EnableResize && !w.OverrideResize)
					b = b.ResizeImage(new Size(ResizeWidth, ResizeHeight), DefaultResizeMode, UseAspectRatio);

				if (EnableCrop && !w.OverrideCrop)
					b = b.CropImage(new Size(CropWidth, CropHeight), DefaultCropAlignment);

				if (EnableWatermark && !w.OverrideWatermark)
					b.WatermarkImage(WatermarkAlignment, (float)WatermarkOpacity, DefaultWatermarkType, WatermarkText, WatermarkFont, WatermarkImagePath, WatermarkGreyscale);

				if (!w.OverrideColor)
					b = b.ColorImage((float)ColorSaturation, ColorType, ColorContrast, ColorBrightness, (float)ColorGamma);

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
					var myEncoderParameter = new EncoderParameter(myencoder, (long)(JpegQuality * 100));
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

			_namingMutex.WaitOne();
			if (System.IO.File.Exists(outpath))
			{
				var i = 0;
				while (System.IO.File.Exists(string.Format(outpathFormat, ++i))) ;
				outpath = string.Format(outpathFormat, i);
			}

			var ret = System.IO.File.Create(outpath);
			_namingMutex.ReleaseMutex();
			return ret;
		}

		private string GenerateFilename(IFile w, Image b)
		{
			string name = null;
			switch (NameOption)
			{
				case NameType.Original:
					name = w.Name;
					break;
				case NameType.Numbered:
					name = w.ImageNumber.ToString(CultureInfo.InvariantCulture);
					break;
				case NameType.Custom:
					name = OutputTemplate;
					name = name.Replace("{o}", w.Name);
					name = name.Replace("{w}", b.Width.ToString(CultureInfo.InvariantCulture));
					name = name.Replace("{h}", b.Height.ToString(CultureInfo.InvariantCulture));
					break;
			}
			return name;
		}
	}
}