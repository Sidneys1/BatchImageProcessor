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
using System.ComponentModel;

namespace BatchImageProcessor.Model.Types.Enums
{
	public enum InterpolationMode
	{
		Default,
		Grayscale,
		[Description("Greyscale (Unscaled Pixel Values)")]
		GrayscaleUnscaled,
		[Description("Greyscale (Unscaled Pixel Values, Don't Crop Masked Pixels")]
		GrayscaleUnscaledNoCrop,
		[Description("Halfsized Color Image")]
		Halfsize,
		[Description("Bilinear Interpolation")]
		Q0,
		[Description("VNG (Variable Number of Gradients) Interpolation")]
		Q1,
		[Description("PPG (Patterned Pixel Grouping) Interpolation")]
		Q2,
		[Description("AHD (Adaptive Homogeneity-Directed) Interpolation")]
		Q3,
		[Description("Interpolate RGB as Four Colors")]
		FourColor
	}
}