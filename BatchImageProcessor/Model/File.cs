using Microsoft.WindowsAPICodePack.Shell;
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
	public class File : IoObject
	{
		#region Properties

		WeakThumbnail _thumb = null;
		public override ImageSource Thumbnail
		{
			get { return _thumb != null ? _thumb.Source : (_thumb = new WeakThumbnail(Path)).Source; }
		}

		public Rotation RotationTransform { get; set; }

		#endregion

		public File(string path)
			: base(path)
		{

		}
	}
}
