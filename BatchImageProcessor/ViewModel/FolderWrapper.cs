using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BatchImageProcessor.Annotations;
using BatchImageProcessor.Model;
using BatchImageProcessor.Model.Interface;
using BatchImageProcessor.Properties;

namespace BatchImageProcessor.ViewModel
{
	public class FolderWrapper : INotifyPropertyChanged, IFolderable, IFolderableHost, IEditableObject
	{
		private readonly Folder _folder;

		public FolderWrapper(string fromPath = null, bool recursion = true, bool removable = true)
		{
			_folder = new Folder();

			if (fromPath != null)
				Populate(fromPath, recursion);
			Removable = removable;
			_folder.Name = Resources.New_Folder_Name;
		}

		public bool Removable { get; private set; }

		public string Name
		{
			get { return _folder.Name; }
			set
			{
				if (_folder.Name.Equals(value, StringComparison.Ordinal)) return;
				
				_folder.Name = value;
				PropChanged(nameof(Name));
			}
		}

		public ObservableCollection<IFolderable> Files => _folder.Files;
		
		private void Populate(string path, bool recursive)
		{
			var info = new DirectoryInfo(path);

			Name = info.Name;

			if (recursive)
			{
				var folders = info.GetDirectories();

				foreach (var inf in folders)
				{
					_folder.Files.Add(new FolderWrapper(inf.FullName));
				}
			}

			foreach (var inf in new[] { "*.jpg", "*.jpeg", "*.png" }.Select(str => info.GetFiles(str)).SelectMany(files => files))
			{
				_folder.Files.Add(new FileWrapper(inf.FullName));
			}
		}

		public bool ContainsFile(string p)
		{
			return _folder.ContainsFile(p);
		}

		#region IEditableObject

		private string _backupName;

		public void BeginEdit()
		{
			_backupName = Name;
		}

		public void EndEdit()
		{
			_backupName = null;
			Name = Name.Trim();
			Name = Folder.InvalidCharacters.ToList().Aggregate(Name, (x, y) => x.Replace(y.ToString(), ""));
		}

		public void CancelEdit()
		{
			if (_backupName != null) Name = _backupName;
		}

		#endregion

		public void AddFile(string str)
		{
			Files.Add(new FileWrapper(str));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void PropChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}