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