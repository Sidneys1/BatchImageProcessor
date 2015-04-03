using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using BatchImageProcessor.Model;
using BatchImageProcessor.Properties;

namespace BatchImageProcessor.ViewModel
{
    public class Folder : INotifyPropertyChanged, IFolderable
    {
        private string _name = Resources.New_Folder_Name;
	    private static readonly ISet<char> InvalidCharacters = new HashSet<char>(Path.GetInvalidPathChars());
		//public WeakThumbnail Thumbnail { get; protected set; }

		public Folder(string fromPath = null, bool recursion = true, bool removable = true)
        {
            IsValidName = true;
            Files = new ObservableCollection<IFolderable>();
            if (fromPath != null)
                Populate(fromPath, recursion);
            Removable = removable;
			//Thumbnail = new WeakThumbnail(Directory.GetCurrentDirectory());
			//Thumbnail.SourceUpdated += Thumbnail_SourceUpdated;
        }

		//private void Thumbnail_SourceUpdated()
		//{
		//	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Thumbnail"));
		//}

		public ObservableCollection<IFolderable> Files { get; }
        public bool Removable { get; private set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name.Equals(value, StringComparison.Ordinal)) return;

	            var n = value.Trim();
				if (n.Any(InvalidCharacters.Contains) || string.IsNullOrWhiteSpace(n) )
                {
                    IsValidName = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsValidName"));
                    throw new Exception("Data Validation Error");
                }

                _name = n;
                IsValidName = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsValidName"));
            }
        }

        public bool IsValidName { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

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

            foreach (var inf in new[] {"*.jpg", "*.jpeg", "*.png"}.Select(str => info.GetFiles(str)).SelectMany(files => files))
            {
                Files.Add(new FileWrapper(inf.FullName));
            }
        }

        internal bool ContainsFile(string p)
        {
            return
                Files.Any(
                    o =>
                        (o is Folder)
                            ? string.Equals(((Folder) o).Name, p, StringComparison.Ordinal)
                            : string.Equals(((FileWrapper) o).Name, p, StringComparison.Ordinal));
        }

        public Folder FindParent(Folder f)
        {
            return Files.Contains(f)
                ? this
                : (from Folder p in Files select p.FindParent(f)).FirstOrDefault(ret => ret != null);
        }
    }
}