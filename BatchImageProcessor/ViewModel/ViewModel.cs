using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchImageProcessor.ViewModel
{
	public class ViewModel : INotifyPropertyChanged, IDisposable
	{
		#region Properties

		public ObservableCollection<Folder> Folders { get; private set; }

		#region Rotate Settings

		Model.Rotation _defaultRotation = Model.Rotation.None;
		public Model.Rotation DefaultRotation { get { return _defaultRotation; } set { _defaultRotation = value; PropChanged("DefaultRotation"); } } 

		#endregion

		#region Crop Settings

		Alignment _defaultCropAlignment = Alignment.Middle_Center;
		public Alignment DefaultCropAlignment { get { return _defaultCropAlignment; } set { _defaultCropAlignment = value; PropChanged("DefaultCropAlignment"); } } 

		#endregion

		#region Resize Settings

		Model.ResizeMode _defaultResizeMode = Model.ResizeMode.Smaller;
		public Model.ResizeMode DefaultResizeMode { get { return _defaultResizeMode; } set { _defaultResizeMode = value; PropChanged("DefaultResizeMode"); } }

		bool _useAspectRatio = true;
		public bool UseAspectRatio { get { return _useAspectRatio; } set { _useAspectRatio = value; PropChanged("UseAspectRatio"); } }

		int _resizeWidth = 800;
		public int ResizeWidth { get { return _resizeWidth; } set { _resizeWidth = value; PropChanged("ResizeWidth"); } }

		int _resizeHeight = 600;
		public int ResizeHeight { get { return _resizeHeight; } set { _resizeHeight = value; PropChanged("ResizeHeight"); } }

		#endregion

		#region Watermark Settings

		Model.WatermarkType _defaultWatermarkType = Model.WatermarkType.Text;
		public Model.WatermarkType DefaultWatermarkType { get { return _defaultWatermarkType; } set { _defaultWatermarkType = value; PropChanged("DefaultWatermarkType"); } }

		string _watermarkText = "Watermark Text";
		public string WatermarkText { get { return _watermarkText; } set { _watermarkText = value; PropChanged("WatermarkText"); } }

		Font _watermarkFont = new Font("Calibri", 12f);
		public Font WatermarkFont { get { return _watermarkFont; } set { _watermarkFont = value; PropChanged("WatermarkFont"); PropChanged("WatermarkFontString"); } }

		public string WatermarkFontString 
		{
			get 
			{
				return string.Format("{0}, {1}pt", WatermarkFont.FontFamily.Name, WatermarkFont.SizeInPoints);
			}
		}

		double _watermarkOpacity = 0.7;
		public double WatermarkOpacity { get { return _watermarkOpacity; } set { _watermarkOpacity = value; PropChanged("WatermarkOpacity"); } }

		Alignment _watermarkAlignment = Alignment.Bottom_Right;
		public Alignment WatermarkAlignment { get { return _watermarkAlignment; } set { _watermarkAlignment = value; PropChanged("WatermarkAlignment"); } }

		string _watermarkImagePath = "<No File Set>";
		public string WatermarkImagePath { get { return _watermarkImagePath; } set { _watermarkImagePath = value; PropChanged("WatermarkImagePath"); } }

		bool _watermarkGreyscale = true;
		public bool WatermarkGreyscale { get { return _watermarkGreyscale; } set { _watermarkGreyscale = value; PropChanged("WatermarkGreyscale"); } }

		#endregion

		#region OutputSettings

		string _outputPath = "<No Path Set>";
		public string OutputPath { get { return _outputPath; } set { _outputPath = value; PropChanged("OutputPath"); } }

		bool _outputSet = false;
		public bool OutputSet { get { return _outputSet; } set { _outputSet = value; PropChanged("OutputSet"); } }

		Model.NameType _nameOption = Model.NameType.Original;
		public Model.NameType NameOption { get { return _nameOption; } set { _nameOption = value; PropChanged("NameOption"); } }

		string _outputTemplate = "{o} - Processed";
		public string OutputTemplate { get { return _outputTemplate; } set { _outputTemplate = value; PropChanged("OutputTemplate"); PropChanged("OutputTemplateExample"); } }
		public string OutputTemplateExample
		{
			get 
			{
				string str = OutputTemplate.Trim() + ".jpg";
				str = str.Replace("{o}", "DSCF3013");
				str = str.Replace("{w}", "1920");
				str = str.Replace("{h}", "1080");
				return str;
			}
		}
		#endregion

		#region Checkboxes

		bool _enableRotation = false;
		public bool EnableRotation { get { return _enableRotation; } set { _enableRotation = value; PropChanged("EnableRotation"); } }

		bool _enableCrop = false;
		public bool EnableCrop { get { return _enableCrop; } set { _enableCrop = value; PropChanged("EnableCrop"); } }

		bool _enableResize = false;
		public bool EnableResize { get { return _enableResize; } set { _enableResize = value; PropChanged("EnableResize"); } }

		bool _enableWatermark = false;
		public bool EnableWatermark { get { return _enableWatermark; } set { _enableWatermark = value; PropChanged("EnableWatermark"); } }

		#endregion

		#endregion

		public ViewModel()
		{
			Folders = new ObservableCollection<Folder>();
		}

		#region Property Changed Stuff

		public void PropChanged(string val)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(val));
		}

		public event PropertyChangedEventHandler PropertyChanged; 

		#endregion

		#region Folder Removal Overloads

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

		#endregion

		public void Dispose()
		{
			_watermarkFont.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}