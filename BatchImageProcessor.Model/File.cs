using BatchImageProcessor.Model.Interface;
using BatchImageProcessor.Model.Types;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model
{
	public class File : IoObject, IFolderable, IFile
	{
		#region Variables

		public OptionSet Options { get; set; } = new OptionSet {Rotation = Rotation.Default};
		public bool Selected { get; set; } = true;

		public RawOptions RawOptions { get; set; } = null;
		public bool IsRaw { get; set; } = false;

		#endregion

		public File(string path) : base(path)
        {
		Options.OutputOptions.OutputFormat = Format.Default;
        }

        #region Properties

	    
	    public int ImageNumber { get; set; }
	    public string OutputPath { get; set; }

	    #endregion
	}
}