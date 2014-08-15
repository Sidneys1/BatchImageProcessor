using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchImageProcessor.ViewModel
{
	public class ViewModel
	{
		public ObservableCollection<Folder> Folders { get; private set; }

		public ViewModel()
		{
			Folders = new ObservableCollection<Folder>();
		}
	}
}
