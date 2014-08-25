using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BatchImageProcessor.Model
{
	public abstract class IoObject : INotifyPropertyChanged, IDisposable
	{
		string _path = null;
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

		string _name = null;
		public string Name 
		{
			get
			{
				return _name ?? (_name = GetName(Path));
			}
		}

		FileSystemWatcher Watcher = null;

		public bool IsFile { get; set; }

		public abstract ImageSource Thumbnail { get; }

		public IoObject(string path)
		{
			bool file = System.IO.File.Exists(path);
			if (!(file || System.IO.Directory.Exists(path)))
				throw new FileNotFoundException(string.Format(@"File/folder at ""{0}"" does not exist.", path));

			IsFile = file;
			Path = path;

			Watcher = new FileSystemWatcher(IsFile ? new FileInfo(Path).Directory.FullName : Path);
			if (IsFile)
				Watcher.Filter = System.IO.Path.GetFileName(path);

			Watcher.Renamed += watcher_Renamed;
			Watcher.Deleted += Watcher_Deleted;
			Watcher.EnableRaisingEvents = true;
		}

		void Watcher_Deleted(object sender, FileSystemEventArgs e)
		{
			
		}

		void watcher_Renamed(object sender, RenamedEventArgs e)
		{
			if (IsFile)
			{
				Path = e.FullPath;
				Watcher.Filter = e.Name;
			}
		}

		public IoObject() { }

		public static string GetName(string Path)
		{
			if (Directory.Exists(Path) && (System.IO.File.GetAttributes(Path) & FileAttributes.Directory) == FileAttributes.Directory)
				return System.IO.Path.GetFileName(Path);
			else if (System.IO.File.Exists(Path))
				return System.IO.Path.GetFileNameWithoutExtension(Path);
			else
				throw new FileNotFoundException(string.Format(@"File/folder at ""{0}"" does not exist.", Path));
		}

		public void PropChanged(string PropertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void Dispose()
		{
			Watcher.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
