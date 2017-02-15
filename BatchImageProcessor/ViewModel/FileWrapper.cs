using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BatchImageProcessor.Annotations;
using BatchImageProcessor.Model;
using BatchImageProcessor.Model.Interface;
using BatchImageProcessor.Model.Types;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.ViewModel
{
	public class FileWrapper : INotifyPropertyChanged, IFolderable, IFile, IEditableObject
	{
		private readonly File _file;

		public FileWrapper(string path) // : base(path)
		{
			_file = new File(path);
			Thumbnail = new WeakThumbnail(path);
			Thumbnail.SourceUpdated += () => { PropChanged(nameof(Thumbnail)); };
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

		public OptionSet Options
		{
			get { return _file.Options; }
			set { _file.Options = value; PropChanged(); }
		}

		public Rotation OverrideRotation
		{
			get { return _file.Options.Rotation; }
			set
			{
				_file.Options.Rotation = value;
				PropChanged();
			}
		}

		public int ImageNumber { get; set; }

		public string OutputPath { get; set; }

		public Format OverrideFormat
		{
			get { return _file.Options.OutputOptions.OutputFormat; }
			set
			{
				_file.Options.OutputOptions.OutputFormat = value;
				PropChanged();
			}
		}

		public bool OverrideResize
		{
			get { return _file.Options.EnableResize; }
			set
			{
				_file.Options.EnableResize = value;
				PropChanged();
			}
		}

		public bool OverrideCrop
		{
			get { return _file.Options.EnableCrop; }
			set
			{
				_file.Options.EnableCrop = value;
				PropChanged();
			}
		}

		public bool OverrideWatermark
		{
			get { return _file.Options.EnableWatermark; }
			set
			{
				_file.Options.EnableWatermark = value;
				PropChanged();
			}
		}

		public bool OverrideColor
		{
			get { return _file.Options.EnableAdjustments; }
			set
			{
				_file.Options.EnableAdjustments = value;
				PropChanged();
			}
		}

		public string Name
		{
			get { return _file.Name; }
			set
			{
				if (_file.Name.Equals(value, StringComparison.Ordinal)) return;
				
				_file.Name = value;
				PropChanged();
				
			}
		}
		
		public string Path => _file.Path;

		public bool IsFile => true;

		public WeakThumbnail Thumbnail { get; }

		public RawOptions RawOptions {get { return _file.RawOptions; } set { _file.RawOptions = value; PropChanged();} }
		public bool IsRaw {
			get { return _file.IsRaw; }
			set { _file.IsRaw = value; PropChanged(); }
		}

		#endregion
		

	    public event PropertyChangedEventHandler PropertyChanged;

	    [NotifyPropertyChangedInvocator]
	    protected virtual void PropChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }

		#region IEditableObject

		private string _backupName;
		private RawOptions _rawBackup;
		private OptionSet _optionsBackup;
		public void BeginEdit()
		{
			_backupName = Name;
			if (_file.RawOptions != null)
				_rawBackup = Model.Utility.ObjectCopier.Clone(_file.RawOptions);
			if (_file.Options != null)
				_optionsBackup = Model.Utility.ObjectCopier.Clone(_file.Options);
		}

		public void EndEdit()
		{
			_backupName = null;
			_rawBackup = null;
			_optionsBackup = null;
			Name = Name.Trim();
			Name = File.InvalidCharacters.ToList().Aggregate(Name, (x, y) => x.Replace(y.ToString(), ""));
		}

		public void CancelEdit()
		{
			if (_backupName != null) Name = _backupName;
			if (_rawBackup != null) RawOptions = _rawBackup;
			if (_optionsBackup != null) Options = _optionsBackup;
			EndEdit();
		}

		#endregion
	}
}