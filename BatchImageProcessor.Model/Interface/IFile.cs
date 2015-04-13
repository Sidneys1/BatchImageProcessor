using BatchImageProcessor.Model.Types;

namespace BatchImageProcessor.Model.Interface
{
	public interface IFile : IIoObject
	{
		bool Selected { get; set; }

		#region Override Toggles

		//bool OverrideCrop { get; set; }
		//bool OverrideColor { get; set; }
		//bool OverrideResize { get; set; }
		//bool OverrideWatermark { get; set; }
		//Format OverrideFormat { get; set; }
		//Rotation OverrideRotation { get; set; }

		#endregion
		
		OptionSet Options { get; set; }


		int ImageNumber { get; set; }
		string OutputPath { get; set; }
		bool IsRaw { get; set; }
		RawOptions RawOptions { get; set; }
	}
}