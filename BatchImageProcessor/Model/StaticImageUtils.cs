using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using BatchImageProcessor.Types;
using Rotation = BatchImageProcessor.Types.Rotation;

namespace BatchImageProcessor.Model
{
	public static class StaticImageUtils
	{
		public static ImageCodecInfo GetEncoder(ImageFormat format)
		{
			var imageCodecInfo = ImageCodecInfo.GetImageDecoders();
			return imageCodecInfo.FirstOrDefault(codec => codec.FormatID == format.Guid);
		}

		public static Image GetRawImageData(string path, string execDcrawExe)
		{
			var f = new FileInfo(execDcrawExe);
			var startInfo = new ProcessStartInfo(f.FullName)
			{
				Arguments = "-c -T -W \"" + path + "\"",
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			var process = Process.Start(startInfo);

			if (process == null) return null;

			try
			{
				return Image.FromStream(process.StandardOutput.BaseStream, true, true);
			}
			catch
			{
				// ignored
			}

			return null;
		}

		#region Image Processing

		public static void RotateImage(this Image b, Rotation rotation)
		{
			switch (rotation)
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

		public static Image ResizeImage(this Image b, Size size, ResizeMode resizeMode, bool useAspectRatio)
		{
			var newSize = b.Size;
			var targetSize = size;

			if (useAspectRatio)
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

			switch (resizeMode)
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

		public static Image CropImage(this Image b, Size size, Alignment defaultCropAlignment)
		{
			var cropSize = size;

			if (cropSize.Width > b.Width || cropSize.Height > b.Height)
				cropSize = new Size(
					cropSize.Width > b.Width ? b.Width : cropSize.Width,
					cropSize.Height > b.Height ? b.Height : cropSize.Height);

			int x = 0, y = 0;

			switch (defaultCropAlignment)
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

			switch (defaultCropAlignment)
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

		public static void WatermarkImage(this Image b, Alignment watermarkAlignment, float watermarkOpacity,
			WatermarkType defaultWatermarkType, string watermarkText, Font watermarkFont, string watermarkImagePath,
			bool watermarkGreyscale)
		{
			using (var g = Graphics.FromImage(b))
			{
				var align = watermarkAlignment;
				var opac = watermarkOpacity;
				var margin = new SizeF(20, 20);
				if (defaultWatermarkType == WatermarkType.Text)
				{
					var text = watermarkText;
					var font = watermarkFont;

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
							tPos.Y = (b.Height/2f) - (tSize.Height/2f);
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
							tPos.X = (b.Width/2f) - (tSize.Width/2f);
							break;
						case Alignment.Top_Right:
						case Alignment.Middle_Right:
						case Alignment.Bottom_Right:
							tPos.X = b.Width - margin.Width - tSize.Width;
							break;
					}

					using (var brush = new SolidBrush(Color.FromArgb((int) (255*opac), Color.White)))
					{
						g.SmoothingMode = SmoothingMode.HighQuality;
						g.TextRenderingHint = TextRenderingHint.AntiAlias;
						g.DrawString(text, font, brush, tPos.X, tPos.Y);
						brush.Color = Color.FromArgb((int) (255*opac), Color.Black);
						g.DrawString(text, font, brush, tPos.X - (font.SizeInPoints/24) - 1, tPos.Y - (font.SizeInPoints/24) - 1);
					}
				}
				else
				{
					var path = watermarkImagePath;

					if (!System.IO.File.Exists(path))
						return;

					var grey = watermarkGreyscale;

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
						new[] {tl, tc, tr, 0f, 0f}, // RED
						new[] {ml, mc, mr, 0f, 0f}, // GREEN
						new[] {bl, bc, br, 0f, 0f}, // BLUE
						new[] {0f, 0f, 0f, 1f, 0f}, // Alpha
						new[] {0, 0, 0, 0f, 1f}
					};

					var imageAttributes = new ImageAttributes();
					imageAttributes.ClearColorMatrix();
					var colorMatrix = new ColorMatrix(ptsArray) {Matrix33 = opac};
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
							tPos.Y = (b.Height/2f) - (i.Height/2f);
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
							tPos.X = (b.Width/2f) - (i.Width/2f);
							break;
						case Alignment.Top_Right:
						case Alignment.Middle_Right:
						case Alignment.Bottom_Right:
							tPos.X = b.Width - margin.Width - i.Width;
							break;
					}

					g.DrawImage(i, new Rectangle((int) tPos.X, (int) tPos.Y, i.Width, i.Height), 0, 0, i.Width, i.Height,
						GraphicsUnit.Pixel, imageAttributes);
				}
			}
		}

		public static Image ColorImage(this Image image, float colorSaturation, ColorType colorType, double colorContrast,
			double colorBrightness, float colorGamma)
		{
			var sat = colorSaturation;

			float tl = 1f, tc = 0f, tr = 0f, ml = 0f, mc = 1f, mr = 0f, bl = 0f, bc = 0f, br = 1f;

			const float rwgt = .3086f;
			const float gwgt = .6094f;
			const float bwgt = .0820f;

			switch (colorType)
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
					tl = (1f - sat)*rwgt + sat;
					tc = tr = (1f - sat)*rwgt;

					mc = (1f - sat)*gwgt + sat;
					ml = mr = (1f - sat)*gwgt;

					br = (1f - sat)*bwgt + sat;
					bl = bc = (1f - sat)*bwgt;
					break;
			}

			tl *= (float) colorContrast;
			mc *= (float) colorContrast;
			br *= (float) colorContrast;

			float[][] ptsArray =
			{
				new[] {tl, tc, tr, 0f, 0f}, // RED
				new[] {ml, mc, mr, 0f, 0f}, // GREEN
				new[] {bl, bc, br, 0f, 0f}, // BLUE
				new[] {0f, 0f, 0f, 1f, 0f}, // Alpha
				new[] {(float) colorBrightness - 1f, (float) colorBrightness - 1f, (float) colorBrightness - 1f, 0f, 1f}
			};

			var imageAttributes = new ImageAttributes();
			imageAttributes.ClearColorMatrix();
			imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			imageAttributes.SetGamma(colorGamma, ColorAdjustType.Bitmap);

			using (var g = Graphics.FromImage(image))
			{
				g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height,
					GraphicsUnit.Pixel, imageAttributes);
			}

			return image;
		}

		public static Image LoadImage(string fileName)
		{
			var f = new FileInfo(fileName);
			if (f.Extension.ToUpper() != ".DNG" && f.Extension.ToUpper() != ".NEF")
			{
				return Image.FromFile(fileName, true);
			}
			return GetRawImageData(fileName, ".\\Exec\\dcraw.exe");
		}

		#endregion

		public static BitmapSource GetSource(this Bitmap bit)
		{
			var bitmapImage = new BitmapImage();
			using (var memory = new MemoryStream())
			{
				bit.Save(memory, ImageFormat.Png);
				memory.Position = 0;

				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
			}
			return bitmapImage;
		}
	}
}
