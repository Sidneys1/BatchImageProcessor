using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchImageProcessor.ViewModel
{
	public class ViewModel //: INotifyPropertyChanged
	{
		public ObservableCollection<Folder> Folders { get; private set; }
		public Model.Rotation DefaultRotation { get; set; }
		public Alignment DefaultCropAlignment { get; set; }

		public bool EnableRotation { get; set; }
		public bool EnableCrop { get; set; }

		public ViewModel()
		{
			Folders = new ObservableCollection<Folder>();
			DefaultRotation = Model.Rotation.None;
			DefaultCropAlignment = Alignment.Middle_Center;
			EnableRotation = false;
			EnableCrop = false;
		}

		//public void PropChanged(string val)
		//{
		//	PropertyChanged(this, new PropertyChangedEventArgs(val));
		//}

		//public event PropertyChangedEventHandler PropertyChanged = delegate {};
	}
}
