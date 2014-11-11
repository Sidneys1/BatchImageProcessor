using System;
using System.ComponentModel;
using System.IO;

namespace BatchImageProcessor.Model
{
	public abstract class IoObject : INotifyPropertyChanged, IDisposable
	{
		string _path;
		public string Path
		{
			get { return _path; }
			set
			{
				_path = value;
				_name = GetName(_path);
				PropChanged("Name");
				PropChanged("Path");
			}
		}

		string _name;
		public string Name 
		{
			get
			{
				return _name ?? (_name = GetName(Path));
			}
		}

		readonly FileSystemWatcher _watcher;

		public bool IsFile { get; set; }

		public abstract WeakThumbnail Thumbnail { get; protected set; }

		protected IoObject(string path)
		{
			var file = System.IO.File.Exists(path);
			if (!(file || Directory.Exists(path)))
				throw new FileNotFoundException(string.Format(@"File/folder at ""{0}"" does not exist.", path));

			IsFile = file;
			Path = path;

			var directoryInfo = new FileInfo(Path).Directory;
			if (directoryInfo != null)
				_watcher = new FileSystemWatcher(IsFile ? directoryInfo.FullName : Path);
			if (IsFile)
				_watcher.Filter = System.IO.Path.GetFileName(path);

			_watcher.Renamed += watcher_Renamed;
			_watcher.Deleted += Watcher_Deleted;
			_watcher.EnableRaisingEvents = true;
		}

		void Watcher_Deleted(object sender, FileSystemEventArgs e)
		{
			
		}

		void watcher_Renamed(object sender, RenamedEventArgs e)
		{
			if (IsFile)
			{
				Path = e.FullPath;
				_watcher.Filter = e.Name;
			}
		}

		protected IoObject() { }

		public static string GetName(string path)
		{
			if (Directory.Exists(path) && (System.IO.File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
				return System.IO.Path.GetFileName(path);
			if (System.IO.File.Exists(path))
				return System.IO.Path.GetFileNameWithoutExtension(path);
			throw new FileNotFoundException(string.Format(@"File/folder at ""{0}"" does not exist.", path));
		}

		public void PropChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void Dispose()
		{
			_watcher.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
