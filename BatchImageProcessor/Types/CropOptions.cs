namespace BatchImageProcessor.Types
{
	public class CropOptions
	{
		public Alignment CropAlignment { get; set; } = Alignment.Middle_Center;
		public int CropWidth { get; set; } = 800;
		public int CropHeight { get; set; } = 600;
	}
}