using System;
using System.ComponentModel;
using System.IO;
using BatchImageProcessor.Interface;

namespace BatchImageProcessor.Model
{
	public abstract class IoObject : INotifyPropertyChanged, IDisposable, IIoObject
	{
        private readonly FileSystemWatcher _watcher;
        private string _path;

        protected IoObject(string path)
        {
            var file = System.IO.File.Exists(path);
            if (!(file || Directory.Exists(path)))
                throw new FileNotFoundException($@"File/folder at ""{path}"" does not exist.");

            IsFile = file;
            Path = path;

	        Name = GetName(path);


            var directoryInfo = new FileInfo(Path).Directory;
            if (directoryInfo != null)
                _watcher = new FileSystemWatcher(IsFile ? directoryInfo.FullName : Path);
            if (IsFile)
                _watcher.Filter = System.IO.Path.GetFileName(path);

            _watcher.Renamed += watcher_Renamed;
            _watcher.Deleted += Watcher_Deleted;
            _watcher.EnableRaisingEvents = true;
        }

        protected IoObject()
        {
        }

		public string Name { get; set; }
		

		public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                PropChanged("Name");
                PropChanged("Path");
            }
        }

        //public string Name => _name ?? (_name = GetName(Path));
        public bool IsFile { get; set; }

        public void Dispose()
        {
            _watcher.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
        }

        private void watcher_Renamed(object sender, RenamedEventArgs e)
        {
	        if (!IsFile) return;
	        Path = e.FullPath;
	        _watcher.Filter = e.Name;
        }
		
        public void PropChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		public static string GetName(string path)
		{
			if (Directory.Exists(path) &&
				(System.IO.File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
				return System.IO.Path.GetFileName(path);
			if (System.IO.File.Exists(path))
				return System.IO.Path.GetFileNameWithoutExtension(path);
			throw new FileNotFoundException($@"File/folder at ""{path}"" does not exist.");
		}
	}
}