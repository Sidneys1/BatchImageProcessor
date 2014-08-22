using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace BatchImageProcessor.ViewModel
{
	public class Folder : INotifyPropertyChanged, IFolderable
	{
		public ObservableCollection<IFolderable> Files { get; private set; }

		public bool Removable { get; private set; }

		static Regex nameCheck = null;

		string _name = "New Folder";
		public string Name
		{
			get { return _name; }
			set
			{
				if (!_name.Equals(value))
				{
					Regex containsABadCharacter = nameCheck ?? (nameCheck = new Regex("[" + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]"));
					if (string.IsNullOrWhiteSpace(value) || containsABadCharacter.IsMatch(value))
					{
						IsValidName = false;
						PropertyChanged(this, new PropertyChangedEventArgs("IsValidName"));
						throw new System.Exception("Data Validation Error");
					}

					_name = value;
					IsValidName = true;
					PropertyChanged(this, new PropertyChangedEventArgs("Name"));
					PropertyChanged(this, new PropertyChangedEventArgs("IsValidName"));
				}
			}
		}

		public bool IsValidName { get; private set; }

		public Folder(string fromPath = null, bool recursion = true, bool removable = true)
		{
			IsValidName = true;
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

			foreach (string str in new string[] { "*.jpg", "*.jpeg" })
			{
				var files = info.GetFiles(str);

				foreach (FileInfo inf in files)
				{
					Files.Add(new FileWrapper(inf.FullName));
				}
			}
			
		}

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		internal bool ContainsFile(string p)
		{
			return Files.Any(o => (o is Folder) ? (o as Folder).Name == p : (o as FileWrapper).Name == p);
		}
	}
}
