using BatchImageProcessor.Model;

namespace BatchImageProcessor.ViewModel
{
	public interface IFolderable
	{
	}

	public class FileWrapper : File, IFolderable
	{
		private bool _overrideCrop;
		private bool _overrideResize;
		private bool _overrideWatermark;
		private Rotation _rotOverride = Rotation.Default;
		private bool _selected = true;

		public FileWrapper(string path)
			: base(path)
		{
			Thumbnail.SourceUpdated += Thumbnail_SourceUpdated;
		}

		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (value != Selected)
				{
					_selected = value;
					PropChanged("Selected");
				}
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

		public string OutputPath { get; set; }

		public int ImageNumber { get; set; }

		private void Thumbnail_SourceUpdated()
		{
			PropChanged("Thumbnail");
		}
	}
}