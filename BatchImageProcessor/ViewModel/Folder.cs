using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace BatchImageProcessor.ViewModel
{
	public class Folder : INotifyPropertyChanged, IFolderable
	{
		public ObservableCollection<IFolderable> Files { get; private set; }

		public bool Removable { get; private set; }

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

		public Folder(string fromPath = null, bool recursion = true, bool removable = true)
		{
			Files = new ObservableCollection<IFolderable>();
			if (fromPath!=null)
				Populate(fromPath, recursion);
			Removable = removable;
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
					Files.Add(new Folder(inf.FullName));
				}
			}

			foreach (string str in new string[] { "*.jpg", "*.jpeg", "*.png" })
			{
				var files = info.GetFiles(str);

				foreach (FileInfo inf in files)
				{
					Files.Add(new FileWrapper(inf.FullName));
				}
			}
			
		}

		public event PropertyChangedEventHandler PropertyChanged = delegate { };
	}
}
