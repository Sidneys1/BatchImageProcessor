using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace BatchImageProcessor.ViewModel
{
	public class Folder:INotifyPropertyChanged
	{
		public List<FileWrapper> Files { get; private set; }

		string _name = "New Folder";
		public string Name
		{
			get { return _name; }
			set
			{
				if (!_name.Equals(value))
				{
					_name = value;
					PropertyChanged(this, new PropertyChangedEventArgs("Name"));
				}
			}
		}

		public Folder()
		{
			Files = new List<FileWrapper>();
		}

		public Folder(string fromPath, bool recursion = true)
		{
			Files = new List<FileWrapper>();
			Populate(fromPath, recursion);
		}

		private void Populate(string Path, bool recursive)
		{
			var info = new DirectoryInfo(Path);

			Name = info.Name;

			if (recursive)
			{
				var folders = info.GetDirectories();

				foreach (DirectoryInfo inf in folders)
				{
					//Files.Add(new Folder(inf.FullName));
				}
			}

			var files = info.GetFiles("*.jpg");

			foreach (FileInfo inf in files)
			{
				Files.Add(new FileWrapper(inf.FullName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged = delegate { };
	}
}
