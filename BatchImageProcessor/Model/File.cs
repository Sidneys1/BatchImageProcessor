using System.Runtime.Serialization.Formatters;
using BatchImageProcessor.Interface;
using BatchImageProcessor.Types;

namespace BatchImageProcessor.Model
{
	public class File : IoObject, IFolderable, IFile
	{
	    #region Variables
		
	    public bool Selected { get; set; } = true;
	    public bool OverrideCrop { get; set; } = false;
	    public bool OverrideColor { get; set; } = false;
	    public bool OverrideResize { get; set; } = false;
	    public bool OverrideWatermark { get; set; } = false;
	    public Format OverrideFormat { get; set; } = Format.Default;
	    public Rotation OverrideRotation { get; set; } = Rotation.Default;

	    #endregion
		
	    public File(string path) : base(path)
        {
        }

        #region Properties

	    
	    public int ImageNumber { get; set; }
	    public string OutputPath { get; set; }

	    #endregion
    }
}