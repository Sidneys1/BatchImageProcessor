using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BatchImageProcessor.Model
{
	public class Folder : IoObject
	{
		#region Properties

		public ObservableCollection<IoObject> Children { get; private set; }

		static WeakThumbnail _thumb = null;

		public override ImageSource Thumbnail
		{
			get { return _thumb != null ? _thumb.Source : (_thumb = new WeakThumbnail(Path)).Source; }
		} 

		#endregion

		public Folder(string path, bool recursive = true)
			: base(path)
		{
			Children = new ObservableCollection<IoObject>();

			Populate(recursive);
		}

		public Folder()
		{

		}

		private void Populate(bool recursive)
		{
			var info = new DirectoryInfo(Path);

			if (recursive)
			{
				var folders = info.GetDirectories();

				foreach (DirectoryInfo inf in folders)
				{
					Children.Add(new Folder(inf.FullName));
				}
			}

			var files = info.GetFiles("*.jpg");

			foreach (FileInfo inf in files)
			{
				Children.Add(new File(inf.FullName));
			}
		}
	}
}
