using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchImageProcessor.ViewModel
{
	public class ViewModel : INotifyPropertyChanged
	{
		#region Properties

		public ObservableCollection<Folder> Folders { get; private set; }

		Model.Rotation _defaultRotation = Model.Rotation.None;
		public Model.Rotation DefaultRotation { get { return _defaultRotation; } set { _defaultRotation = value; PropChanged("DefaultRotation"); } }

		Alignment _defaultCropAlignment = Alignment.Middle_Center;
		public Alignment DefaultCropAlignment { get { return _defaultCropAlignment; } set { _defaultCropAlignment = value; PropChanged("DefaultCropAlignment"); } }

		bool _enableRotation = false;
		public bool EnableRotation { get { return _enableRotation; } set { _enableRotation = value; PropChanged("EnableRotation"); } }

		bool _enableCrop = false;
		public bool EnableCrop { get { return _enableCrop; } set { _enableCrop = value; PropChanged("EnableCrop"); } } 

		#endregion

		public ViewModel()
		{
			Folders = new ObservableCollection<Folder>();
			//DefaultRotation = Model.Rotation.None;
			//DefaultCropAlignment = Alignment.Middle_Center;
			//EnableRotation = false;
			//EnableCrop = false;
		}

		public void PropChanged(string val)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(val));
		}

		public event PropertyChangedEventHandler PropertyChanged;// = delegate { };

		internal void RemoveFolder(Folder folder)
		{
			foreach (Folder parent in Folders)
			{
				if (parent == folder)
				{
					Folders.Remove(folder);
					break;
				}

				if (RemoveFolder(parent, folder))
					break;
			}
		}

		private bool RemoveFolder(Folder parent, Folder folder)
		{
			if (parent.Files.Contains(folder))
			{
				parent.Files.Remove(folder);

				return true;
			}

			foreach (Folder p in parent.Files)
			{
				if (RemoveFolder(p, folder))
					return true;
			} 
			return false;
		}
	}
}
