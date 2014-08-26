using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace BatchImageProcessor.Model
{
	public static class ExtensionMethods
	{
		public static BitmapSource GetSource(this Bitmap bit)
		{
BitmapImage bitmapImage = new BitmapImage();
			using (MemoryStream memory = new MemoryStream())
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
