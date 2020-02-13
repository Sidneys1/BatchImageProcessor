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
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model.Types
{
	[Serializable]
	public class RawOptions
	{
		#region Color Options

		public WhiteBalanceMode WhiteBalance { get; set; }
		public int WhiteBalanceLeft { get; set; }
		public int WhiteBalanceTop { get; set; }
		public int WhiteBalanceWidth { get; set; } = 1;
		public int WhiteBalanceHeight { get; set; } = 1;

		public double WhiteBalanceMul0 { get; set; }
		public double WhiteBalanceMul1 { get; set; }
		public double WhiteBalanceMul2 { get; set; }
		public double WhiteBalanceMul3 { get; set; }

		#endregion

		#region Interpolation Options

		public InterpolationMode InterpolationMode { get; set; }
		public bool Cleanup { get; set; } = false;
		public int CleanupPasses { get; set; } = 1;

		#endregion

		#region Output Options

		public bool FixedWhiteLevel { get; set; } = false;
		public double Brighness { get; set; } = 1.0;

		public bool DisableAutoOrient { get; set; } = false;
		public bool NoStretch { get; set; } = false;

		public string Info { get; set; }

		#endregion

		public RawOptions(string info)
		{
			Info = info;
		}
	}
}
