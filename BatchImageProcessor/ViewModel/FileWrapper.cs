using System.ComponentModel;

namespace BatchImageProcessor.ViewModel
{
	public class FileWrapper : Model.File
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

		public FileWrapper(string path)
			: base(path)
		{ }
	}
}
