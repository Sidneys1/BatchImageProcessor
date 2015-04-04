using System.ComponentModel;
using System.Runtime.CompilerServices;
using BatchImageProcessor.Annotations;
using BatchImageProcessor.Interface;
using BatchImageProcessor.Model;
using BatchImageProcessor.Types;

namespace BatchImageProcessor.ViewModel
{
    public class FileWrapper : INotifyPropertyChanged, IFolderable, IFile
    {
	    private readonly File _file;

	    public FileWrapper(string path)// : base(path)
        {
			_file = new File(path);
			Thumbnail = new WeakThumbnail(path);
		    Thumbnail.SourceUpdated += () => { PropChanged("Thumbnail"); };
		    Name = IoObject.GetName(path);
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

	    public int ImageNumber { get; set; }

	    public string OutputPath { get; set; }

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

	    public bool IsFile => true;

	    public WeakThumbnail Thumbnail { get; }

	    #endregion
		

	    public event PropertyChangedEventHandler PropertyChanged;

	    [NotifyPropertyChangedInvocator]
	    protected virtual void PropChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
    }
}