using System.ComponentModel;

namespace BatchImageProcessor.ViewModel
{
	public interface IFolderable
	{

	}

	public class FileWrapper : Model.File, IFolderable
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

		Model.Rotation _rotOverride = Model.Rotation.Default;
		public Model.Rotation RotationOverride
		{
			get { return _rotOverride; }
			set
			{
				_rotOverride = value;
				PropChanged("RotationOverride");
			}
		}

		public FileWrapper(string path)
			: base(path)
		{
		}
	}
}
