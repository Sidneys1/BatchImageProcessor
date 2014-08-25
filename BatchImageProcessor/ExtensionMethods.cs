using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace BatchImageProcessor
{
	public static class ExtensionMethods
	{
		public static BitmapSource GetSource(this Bitmap bit)
		{
			//return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bit.GetHbitmap(Color.Transparent), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bit.Width, bit.Height));

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

		public static string FormatWith(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
	}
}
