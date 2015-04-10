using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model.Types
{
	public class ResizeOptions
	{
		public ResizeMode ResizeMode { get; set; } = ResizeMode.Smaller;
		public int ResizeWidth { get; set; } = 800;
		public int ResizeHeight { get; set; } = 600;
		public bool UseAspectRatio { get; set; } = true;
	}
}