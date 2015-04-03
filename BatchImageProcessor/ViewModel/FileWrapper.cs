using System.IO;
using BatchImageProcessor.Model;
using File = BatchImageProcessor.Model.File;

namespace BatchImageProcessor.ViewModel
{
    public class FileWrapper : File, IFolderable
    {
        private bool _overrideCrop;
        private bool _overrideResize;
        private bool _overrideWatermark;
	    private bool _overrideColor;
        private Rotation _rotOverride = Rotation.Default;
        private bool _selected = true;
	    private Format _formatOverride;
	    private double _jpegQualityOverride;
	    private string _name;

	    public FileWrapper(string path)
            : base(path)
        {
            Thumbnail.SourceUpdated += Thumbnail_SourceUpdated;
		    _name = GetName(path);
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value == Selected) return;
                _selected = value;
                PropChanged("Selected");
            }
        }

        public Rotation RotationOverride
        {
            get { return _rotOverride; }
            set
            {
                _rotOverride = value;
                PropChanged("RotationOverride");
            }
        }

	    public Format FormatOverride
	    {
		    get { return _formatOverride; }
		    set { _formatOverride = value; PropChanged("FormatOverride");}
	    }

	    public double JpegQualityOverride
	    {
		    get { return _jpegQualityOverride; }
		    set { _jpegQualityOverride = value; PropChanged("JpegQualityOverride");}
	    }

	    public bool OverrideResize
        {
            get { return _overrideResize; }
            set
            {
                _overrideResize = value;
                PropChanged("OverrideResize");
            }
        }

        public bool OverrideCrop
        {
            get { return _overrideCrop; }
            set
            {
                _overrideCrop = value;
                PropChanged("OverrideCrop");
            }
        }

        public bool OverrideWatermark
        {
            get { return _overrideWatermark; }
            set
            {
                _overrideWatermark = value;
                PropChanged("OverrideWatermark");
            }
        }

	    public bool OverrideColor
	    {
			get { return _overrideColor; }
		    set
		    {
			    _overrideColor = value;
			    PropChanged("OverrideColor");
		    }
	    }


        public string OutputPath { get; set; }
        public int ImageNumber { get; set; }

        private void Thumbnail_SourceUpdated()
        {
            PropChanged("Thumbnail");
        }

	    public string Name
	    {
		    get { return _name; }
		    set { _name = value; PropChanged("Name"); }
	    }

	    public static string GetName(string path)
		{
			if (Directory.Exists(path) &&
				(System.IO.File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
				return System.IO.Path.GetFileName(path);
			if (System.IO.File.Exists(path))
				return System.IO.Path.GetFileNameWithoutExtension(path);
			throw new FileNotFoundException(string.Format(@"File/folder at ""{0}"" does not exist.", path));
		}
	}
}