namespace BatchImageProcessor.Model
{
    public class File : IoObject
    {
	    #region Variables

	    public string Name;
		public bool Selected = true;
		public bool OverrideCrop = false;
		public bool OverrideColor = false;
		public bool OverrideResize = false;
		public bool OverrideWatermark = false;
		public Format OverrideFormat = Format.Default;
		public Rotation OverrideRotation = Rotation.Default;
		
	    #endregion
		
	    public File(string path) : base(path)
        {
            Thumbnail = new WeakThumbnail(path);
        }

        #region Properties

	    public WeakThumbnail Thumbnail { get; }
	    public int ImageNumber { get; set; }
	    public string OutputPath { get; set; }

	    #endregion
    }
}