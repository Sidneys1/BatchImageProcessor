using System;
using System.Collections.Generic;

namespace BatchImageProcessor.Types
{
	public struct OptionStruct
	{
		public bool Crop, Watermark, SizeSmart, WatermarkGrey;
		public string Output, Format, WatermarkType, WatermarkText, WatermarkFont;
		public int Rotation, Size, SizeWidth, SizeHeight, CropWidth, CropHeight, CropAlign, WatermarkFontsize, WatermarkAlign, ColorSatMode;
		public double WatermarkOpac, ColorBright, ColorContrast, ColorGamma, ColorSat, OutJpeg;
		public readonly List<string> Files; 

		public OptionStruct()
		{
			Crop = false;
			Watermark = false;
			SizeSmart = true;
			WatermarkGrey = true;
			Output = String.Empty;
			Format = "Jpg";
			WatermarkType = "Text";
			WatermarkText = String.Empty;
			WatermarkFont = "Calibri";
			Rotation = 0;
			SizeWidth = 800;
			Size = 0;
			SizeHeight = 600;
			CropWidth = 800;
			CropHeight = 600;
			CropAlign = 4;
			WatermarkFontsize = 12;
			WatermarkAlign = 8;
			ColorSatMode = 0;
			WatermarkOpac = 0.7;
			ColorBright = 1.0;
			ColorContrast = 1.0;
			ColorGamma = 1.0;
			ColorSat = 1.0;
			OutJpeg = 0.95;
			Files = new List<string>();
		}
	}
}