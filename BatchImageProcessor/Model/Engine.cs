using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
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
		public static bool Cancel = false;

		public static void Process(ViewModel.ViewModel vm)
		{
			_model = vm;
			TotalImages = 0;
			DoneImages = 0;
			int m, n;
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
				var b = Image.FromFile(w.Path, true);
				
				// Process
				if (w.RotationOverride != Rotation.Default || _model.EnableRotation)
					RotateImage(w, b);

				if (_model.EnableResize && !w.OverrideResize)
					b = ResizeImage(b);

				if (_model.EnableCrop && !w.OverrideCrop)
					b = CropImage(b);
				
				if (_model.EnableWatermark && !w.OverrideWatermark)
					WatermarkImage(w, b);

				if (_model.EnableColor && !w.OverrideColor)
					b = ColorImage(b);

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
					while (System.IO.File.Exists(string.Format(outpathFormat, ++i)))
					{
					}
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

		private static Image ColorImage(Image image)
		{
			var sat = (float)_model.ColorSaturation;

			float tl = 1f, tc = 0f, tr = 0f, ml = 0f, mc = 1f, mr = 0f, bl = 0f, bc = 0f, br = 1f;

			const float rwgt = .3086f;
			const float gwgt = .6094f;
			const float bwgt = .0820f;

			switch (_model.ColorType)
			{
				case ColorType.Greyscale:
					tl = tc = tr = rwgt;
					ml = mc = mr = gwgt;
					bl = bc = br = bwgt;
					break;
				case ColorType.Sepia:
					tl = .393f;
					tc = .349f;
					tr = .272f;
					ml = .769f;
					mc = .686f;
					mr = .534f;
					bl = .189f;
					bc = .168f;
					br = .131f;
					break;
				case ColorType.Saturation:
					tl = (1f - sat) * rwgt + sat;
					tc = tr = (1f - sat) * rwgt;

					mc = (1f - sat) * gwgt + sat;
					ml = mr = (1f - sat) * gwgt;

					br = (1f - sat) * bwgt + sat;
					bl = bc = (1f - sat) * bwgt;
					break;
			}

			tl *= (float)_model.ColorContrast;
			mc *= (float)_model.ColorContrast;
			br *= (float)_model.ColorContrast;

			float[][] ptsArray =
			{
				new[] { tl, tc, tr, 0f, 0f}, // RED
				new[] { ml, mc, mr, 0f, 0f }, // GREEN
				new[] { bl, bc, br, 0f, 0f }, // BLUE
				new[] { 0f, 0f, 0f, 1f,0f }, // Alpha
				new[] { (float)_model.ColorBrightness-1f, (float)_model.ColorBrightness-1f, (float)_model.ColorBrightness-1f, 0f, 1f },
			};

			var imageAttributes = new ImageAttributes();
			imageAttributes.ClearColorMatrix();
			imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			imageAttributes.SetGamma((float)_model.ColorGamma, ColorAdjustType.Bitmap);

			using (var g = Graphics.FromImage(image))
			{
				g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				//g.DrawImage(originalImage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), 0, 0, bitImage.Width, bitImage.Height, GraphicsUnit.Pixel, imageAttributes);
			}


			return image;
		}
		
		private static void WatermarkImage(FileWrapper w, Image b)
		{
			if (w == null) throw new ArgumentNullException("w");
			using (var g = Graphics.FromImage(b))
			{
				var align = _model.WatermarkAlignment;
				var opac = (float)_model.WatermarkOpacity;
				var margin = new SizeF(20, 20);
				if (_model.DefaultWatermarkType == WatermarkType.Text)
				{
					var text = _model.WatermarkText;
					var font = _model.WatermarkFont;

					var tSize = g.MeasureString(text, font);

					var tPos = new PointF();

					switch (align)
					{
						case Alignment.Top_Left:
						case Alignment.Top_Center:
						case Alignment.Top_Right:
							tPos.Y = margin.Height;
							break;
						case Alignment.Middle_Left:
						case Alignment.Middle_Center:
						case Alignment.Middle_Right:
							tPos.Y = (b.Height / 2f) - (tSize.Height / 2f);
							break;
						case Alignment.Bottom_Left:
						case Alignment.Bottom_Center:
						case Alignment.Bottom_Right:
							tPos.Y = b.Height - margin.Height - tSize.Height;
							break;
					}

					switch (align)
					{
						case Alignment.Top_Left:
						case Alignment.Middle_Left:
						case Alignment.Bottom_Left:
							tPos.X = margin.Width;
							break;
						case Alignment.Top_Center:
						case Alignment.Middle_Center:
						case Alignment.Bottom_Center:
							tPos.X = (b.Width / 2f) - (tSize.Width / 2f);
							break;
						case Alignment.Top_Right:
						case Alignment.Middle_Right:
						case Alignment.Bottom_Right:
							tPos.X = b.Width - margin.Width - tSize.Width;
							break;
					}

					using (var brush = new SolidBrush(Color.FromArgb((int)(255 * opac), Color.White)))
					{
						g.SmoothingMode = SmoothingMode.HighQuality;
						g.TextRenderingHint = TextRenderingHint.AntiAlias;
						g.DrawString(text, font, brush, tPos.X, tPos.Y);
						brush.Color = Color.FromArgb((int)(255 * opac), Color.Black);
						g.DrawString(text, font, brush, tPos.X - (font.SizeInPoints / 24) - 1, tPos.Y - (font.SizeInPoints / 24) - 1);
					}
				}
				else
				{
					var path = _model.WatermarkImagePath;

					if (!System.IO.File.Exists(path))
						return;

					var grey = _model.WatermarkGreyscale;

					float tl = 1f, tc = 0f, tr = 0f, ml = 0f, mc = 1f, mr = 0f, bl = 0f, bc = 0f, br = 1f;

					const float rwgt = .3086f;
					const float gwgt = .6094f;
					const float bwgt = .0820f;

					if (grey)
					{
						tl = tc = tr = rwgt;
						ml = mc = mr = gwgt;
						bl = bc = br = bwgt;
					}

					float[][] ptsArray =
					{
						new[] { tl, tc, tr, 0f, 0f}, // RED
						new[] { ml, mc, mr, 0f, 0f }, // GREEN
						new[] { bl, bc, br, 0f, 0f }, // BLUE
						new[] { 0f, 0f, 0f, 1f,0f }, // Alpha
						new[] { 0, 0, 0, 0f, 1f },
					};

					var imageAttributes = new ImageAttributes();
					imageAttributes.ClearColorMatrix();
					var colorMatrix = new ColorMatrix(ptsArray) { Matrix33 = opac };
					imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

					var i = Image.FromFile(path);
					var tPos = new PointF();
					switch (align)
					{
						case Alignment.Top_Left:
						case Alignment.Top_Center:
						case Alignment.Top_Right:
							tPos.Y = margin.Height;
							break;
						case Alignment.Middle_Left:
						case Alignment.Middle_Center:
						case Alignment.Middle_Right:
							tPos.Y = (b.Height / 2f) - (i.Height / 2f);
							break;
						case Alignment.Bottom_Left:
						case Alignment.Bottom_Center:
						case Alignment.Bottom_Right:
							tPos.Y = b.Height - margin.Height - i.Height;
							break;
					}

					switch (align)
					{
						case Alignment.Top_Left:
						case Alignment.Middle_Left:
						case Alignment.Bottom_Left:
							tPos.X = margin.Width;
							break;
						case Alignment.Top_Center:
						case Alignment.Middle_Center:
						case Alignment.Bottom_Center:
							tPos.X = (b.Width / 2f) - (i.Width / 2f);
							break;
						case Alignment.Top_Right:
						case Alignment.Middle_Right:
						case Alignment.Bottom_Right:
							tPos.X = b.Width - margin.Width - i.Width;
							break;
					}

					g.DrawImage(i, new Rectangle((int)tPos.X, (int)tPos.Y, i.Width, i.Height), 0, 0, i.Width, i.Height, GraphicsUnit.Pixel, imageAttributes);
				}
			}
		}

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
					y = ((b.Height / 2) - (cropSize.Height / 2));
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
					x = ((b.Width / 2) - (cropSize.Width / 2));
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
				if (b.Width > b.Height)	// landscape
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
						var ratioX = targetSize.Width / (double)b.Width;
						var ratioY = targetSize.Height / (double)b.Height;
						// use whichever multiplier is smaller
						var ratio = ratioX < ratioY ? ratioX : ratioY;

						// now we can get the new height and width
						var newHeight = Convert.ToInt32(b.Height * ratio);
						var newWidth = Convert.ToInt32(b.Width * ratio);

						newSize = new Size(newWidth, newHeight);
					}
					break;
				case ResizeMode.Larger:
					if (b.Width < targetSize.Width || b.Height < targetSize.Height)
					{
						var ratioX = targetSize.Width / (double)b.Width;
						var ratioY = targetSize.Height / (double)b.Height;
						// use whichever multiplier is larger
						var ratio = ratioX > ratioY ? ratioX : ratioY;

						// now we can get the new height and width
						var newHeight = Convert.ToInt32(b.Height * ratio);
						var newWidth = Convert.ToInt32(b.Width * ratio);

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