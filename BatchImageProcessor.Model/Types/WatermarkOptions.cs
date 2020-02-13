// Copyright (c) 2017 Sidneys1
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
using System;
using System.Drawing;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model.Types
{
	[Serializable]
	public class WatermarkOptions
	{
		private Font _watermarkFont;

		public WatermarkOptions()
		{
			WatermarkFont = new Font("Arial", 12f);
		}

		~WatermarkOptions()
		{
			_watermarkFont?.Dispose();
		}

		public bool WatermarkGreyscale { get; set; } = true;
		public string WatermarkText { get; set; } = string.Empty;
		public string WatermarkImagePath { get; set; } = string.Empty;
		public Alignment WatermarkAlignment { get; set; } = Alignment.Bottom_Right;
		public double WatermarkOpacity { get; set; } = 0.7;
		public WatermarkType WatermarkType { get; set; } = WatermarkType.Text;
		public string WatermarkFontString => $"{WatermarkFont.FontFamily.Name}, {WatermarkFont.SizeInPoints}pt";

        public int WatermarkColor { get; set; } = 0;
        

        public Font WatermarkFont
		{
			get { return _watermarkFont; }
			set
			{
				_watermarkFont?.Dispose();
				_watermarkFont = value;
			}
		}
	}
}