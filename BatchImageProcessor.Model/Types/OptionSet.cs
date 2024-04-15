using System;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model.Types
{
	// [System.Runtime.Versioning.SupportedOSPlatform("windows")]
	public class OptionSet
	{
		#region Enabled

		public bool EnableRotation { get; set; } = false;
		public bool EnableResize { get; set; } = false;
		public bool EnableCrop { get; set; } = false;
		public bool EnableWatermark { get; set; } = false;
		public bool EnableAdjustments { get; set; } = false;

		#endregion
		
		public Rotation Rotation { get; set; } = Rotation.None;

		public WatermarkOptions WatermarkOptions { get; } = new WatermarkOptions();

		public OutputOptions OutputOptions { get; } = new OutputOptions();

		public CropOptions CropOptions { get; } = new CropOptions();

		public AdjustmentOptions AdjustmentOptions { get; } = new AdjustmentOptions();

		public ResizeOptions ResizeOptions { get; } = new ResizeOptions();
	}
}