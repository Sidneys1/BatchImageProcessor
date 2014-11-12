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