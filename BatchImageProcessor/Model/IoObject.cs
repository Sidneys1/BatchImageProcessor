using System;
using System.ComponentModel;
using System.IO;

namespace BatchImageProcessor.Model
{
    public abstract class IoObject : INotifyPropertyChanged, IDisposable
    {
        private readonly FileSystemWatcher _watcher;
        private string _name;
        private string _path;

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

        protected IoObject()
        {
        }

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

        public string Name => _name ?? (_name = GetName(Path));
        public bool IsFile { get; set; }
        public abstract WeakThumbnail Thumbnail { get; protected set; }

        public void Dispose()
        {
            _watcher.Dispose();
            GC.SuppressFinalize(this);
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

        public static string GetName(string path)
        {
            if (Directory.Exists(path) &&
                (System.IO.File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                return System.IO.Path.GetFileName(path);
            if (System.IO.File.Exists(path))
                return System.IO.Path.GetFileNameWithoutExtension(path);
            throw new FileNotFoundException(string.Format(@"File/folder at ""{0}"" does not exist.", path));
        }

        public void PropChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}