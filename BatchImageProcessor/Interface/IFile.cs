using BatchImageProcessor.Types;

namespace BatchImageProcessor.Interface
{
	public interface IFile : IIoObject
	{
		bool Selected { get; set; }
		bool OverrideCrop { get; set; }
		bool OverrideColor { get; set; }
		bool OverrideResize { get; set; }
		bool OverrideWatermark { get; set; }
		Format OverrideFormat { get; set; }
		Rotation OverrideRotation { get; set; }
		int ImageNumber { get; set; }
		string OutputPath { get; set; }
		RawOptions RawOptions { get; set; }
		bool IsRaw { get; set; }
	}
}