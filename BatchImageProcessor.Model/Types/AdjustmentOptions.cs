using System;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model.Types
{
	[Serializable]
	public class AdjustmentOptions
	{
		public ColorType ColorType { get; set; } = ColorType.Saturation;
		public double ColorBrightness { get; set; } = 1.0;
		public double ColorContrast { get; set; } = 1.0;
		public double ColorGamma { get; set; } = 1.0;
		public double ColorSaturation { get; set; } = 1.0;
	}
}