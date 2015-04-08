namespace BatchImageProcessor.Types
{
	public class OptionSet
	{


		#region Enabled

		public bool EnableCrop { get; set; } = false;
		public bool EnableWatermark { get; set; } = false;
		public bool EnableRotation { get; set; } = false;
		public bool EnableResize { get; set; } = false;

		#endregion
		
		public Rotation Rotation { get; set; } = Rotation.None;

		public WatermarkOptions WatermarkOptions { get; } = new WatermarkOptions();

		public OutputOptions OutputOptions { get; } = new OutputOptions();

		public CropOptions CropOptions { get; } = new CropOptions();

		public AdjustmentOptions AdjustmentOptions { get; } = new AdjustmentOptions();

		public ResizeOptions ResizeOptions { get; } = new ResizeOptions();
	}
}