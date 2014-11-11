using BatchImageProcessor.Model;

namespace BatchImageProcessor.ViewModel
{
	public interface IFolderable
	{

	}

	public class FileWrapper : File, IFolderable
	{
		bool _selected = true;
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

		Rotation _rotOverride = Rotation.Default;
		public Rotation RotationOverride { get { return _rotOverride; } set { _rotOverride = value; PropChanged("RotationOverride"); } }

		bool _overrideResize;
		public bool OverrideResize { get { return _overrideResize; } set { _overrideResize = value; PropChanged("OverrideResize"); } }

		bool _overrideCrop;
		public bool OverrideCrop { get { return _overrideCrop; } set { _overrideCrop = value; PropChanged("OverrideCrop"); } }

		bool _overrideWatermark;
		public bool OverrideWatermark { get { return _overrideWatermark; } set { _overrideWatermark = value; PropChanged("OverrideWatermark"); } }

		public string OutputPath { get; set; }

		public int ImageNumber { get; set; }

		public FileWrapper(string path)
			: base(path)
		{
			Thumbnail.SourceUpdated += Thumbnail_SourceUpdated;
		}

		void Thumbnail_SourceUpdated()
		{
			PropChanged("Thumbnail");
		}
	}
}
