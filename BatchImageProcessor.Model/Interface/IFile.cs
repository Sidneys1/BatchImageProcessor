using BatchImageProcessor.Model.Types;

namespace BatchImageProcessor.Model.Interface
{
	public interface IFile : IIoObject
	{
		bool Selected { get; set; }
		
		OptionSet Options { get; set; }
		
		int ImageNumber { get; set; }
		string OutputPath { get; set; }

		bool IsRaw { get; set; }
		RawOptions RawOptions { get; set; }
	}
}