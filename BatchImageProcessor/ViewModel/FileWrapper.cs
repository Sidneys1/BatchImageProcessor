using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using BatchImageProcessor.Annotations;
using BatchImageProcessor.Model;
using File = BatchImageProcessor.Model.File;

namespace BatchImageProcessor.ViewModel
{
    public class FileWrapper : IFolderable, INotifyPropertyChanged
    {
	    private readonly File _file;

	    public FileWrapper(string path)// : base(path)
        {
			_file = new File(path);
		    _file.Thumbnail.SourceUpdated += () => { PropChanged("Thumbnail"); };
		    Name = GetName(path);
        }

	    #region Properties
		
	    public bool Selected
	    {
		    get { return _file.Selected; }
		    set
		    {
			    _file.Selected = value;
			    PropChanged();
		    }
	    }

	    public Rotation OverrideRotation
	    {
		    get { return _file.OverrideRotation; }
		    set
		    {
				_file.OverrideRotation = value;
			    PropChanged();
		    }
	    }

	    public Format OverrideFormat
	    {
		    get { return _file.OverrideFormat; }
		    set
		    {
				_file.OverrideFormat = value;
			    PropChanged();
		    }
	    }

	    public bool OverrideResize
	    {
		    get { return _file.OverrideResize; }
		    set
		    {
				_file.OverrideResize = value;
			    PropChanged();
		    }
	    }

	    public bool OverrideCrop
	    {
		    get { return _file.OverrideCrop; }
		    set
		    {
				_file.OverrideCrop = value;
			    PropChanged();
		    }
	    }

	    public bool OverrideWatermark
	    {
		    get { return _file.OverrideWatermark; }
		    set
		    {
				_file.OverrideWatermark = value;
			    PropChanged();
		    }
	    }

	    public bool OverrideColor
	    {
		    get { return _file.OverrideColor; }
		    set
		    {
				_file.OverrideColor = value;
			    PropChanged();
		    }
	    }

	    public string Name
	    {
		    get { return _file.Name; }
		    set
		    {
				_file.Name = value;
			    PropChanged();
		    }
	    }

		public string Path => _file.Path;

	    public WeakThumbnail Thumbnail => _file.Thumbnail;

	    #endregion
		
	    public static string GetName(string path)
		{
			if (Directory.Exists(path) &&
				(System.IO.File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
				return System.IO.Path.GetFileName(path);
			if (System.IO.File.Exists(path))
				return System.IO.Path.GetFileNameWithoutExtension(path);
			throw new FileNotFoundException(string.Format(@"File/folder at ""{0}"" does not exist.", path));
		}

	    public event PropertyChangedEventHandler PropertyChanged;

	    [NotifyPropertyChangedInvocator]
	    protected virtual void PropChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
    }
}