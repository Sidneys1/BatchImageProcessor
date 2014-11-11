using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using BatchImageProcessor.Properties;

namespace BatchImageProcessor.ViewModel
{
	public class Folder : INotifyPropertyChanged, IFolderable
	{
		public ObservableCollection<IFolderable> Files { get; private set; }

		public bool Removable { get; private set; }

		static Regex _nameCheck;

		string _name = Resources.New_Folder_Name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (!_name.Equals(value))
				{
					var containsABadCharacter = _nameCheck ?? (_nameCheck = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]"));
					if (string.IsNullOrWhiteSpace(value) || containsABadCharacter.IsMatch(value))
					{
						IsValidName = false;
						PropertyChanged(this, new PropertyChangedEventArgs("IsValidName"));
						throw new Exception("Data Validation Error");
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

		private void Populate(string path, bool recursive)
		{
			var info = new DirectoryInfo(path);

			Name = info.Name;

			if (recursive)
			{
				var folders = info.GetDirectories();

				foreach (var inf in folders)
				{
					Files.Add(new Folder(inf.FullName));
				}
			}

			foreach (var str in new[] { "*.jpg", "*.jpeg" })
			{
				var files = info.GetFiles(str);

				foreach (var inf in files)
				{
					Files.Add(new FileWrapper(inf.FullName));
				}
			}
			
		}

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		internal bool ContainsFile(string p)
		{
			return Files.Any(o => (o is Folder) ? (o as Folder).Name == p : ((FileWrapper) o).Name == p);
		}

		public Folder FindParent(Folder f)
		{
			return Files.Contains(f) ? this : (from Folder p in Files select p.FindParent(f)).FirstOrDefault(ret => ret != null);
		}
	}
}
