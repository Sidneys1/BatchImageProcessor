using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using BatchImageProcessor.Model;
using BatchImageProcessor.Properties;

namespace BatchImageProcessor.ViewModel
{
	public class ViewModel : INotifyPropertyChanged, IDisposable
	{
		#region Properties

		public ObservableCollection<Folder> Folders { get; private set; }

		#region Rotate Settings

		Rotation _defaultRotation = Rotation.None;
		public Rotation DefaultRotation { get { return _defaultRotation; } set { _defaultRotation = value; PropChanged("DefaultRotation"); } } 

		#endregion

		#region Crop Settings

		Alignment _defaultCropAlignment = Alignment.Middle_Center;
		public Alignment DefaultCropAlignment { get { return _defaultCropAlignment; } set { _defaultCropAlignment = value; PropChanged("DefaultCropAlignment"); } }

		int _cropWidth = 800;
		public int CropWidth { get { return _cropWidth; } set { _cropWidth = value; PropChanged("CropWidth"); } }

		int _cropHeight = 600;
		public int CropHeight { get { return _cropHeight; } set { _cropHeight = value; PropChanged("CropHeight"); } }

		#endregion

		#region Resize Settings

		ResizeMode _defaultResizeMode = ResizeMode.Smaller;
		public ResizeMode DefaultResizeMode { get { return _defaultResizeMode; } set { _defaultResizeMode = value; PropChanged("DefaultResizeMode"); } }

		bool _useAspectRatio = true;
		public bool UseAspectRatio { get { return _useAspectRatio; } set { _useAspectRatio = value; PropChanged("UseAspectRatio"); } }

		int _resizeWidth = 800;
		public int ResizeWidth { get { return _resizeWidth; } set { _resizeWidth = value; PropChanged("ResizeWidth"); } }

		int _resizeHeight = 600;
		public int ResizeHeight { get { return _resizeHeight; } set { _resizeHeight = value; PropChanged("ResizeHeight"); } }

		#endregion

		#region Watermark Settings

		WatermarkType _defaultWatermarkType = WatermarkType.Text;
		public WatermarkType DefaultWatermarkType { get { return _defaultWatermarkType; } set { _defaultWatermarkType = value; PropChanged("DefaultWatermarkType"); } }

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

		string _outputPath = Resources.ViewModel__outputPath__No_Path_Set;
		public string OutputPath { get { return _outputPath; } set { _outputPath = value; PropChanged("OutputPath"); } }

		bool _outputSet;
		public bool OutputSet { get { return _outputSet; } set { _outputSet = value; PropChanged("OutputSet"); PropChanged("Ready"); } }

		NameType _nameOption = NameType.Original;
		public NameType NameOption { get { return _nameOption; } set { _nameOption = value; PropChanged("NameOption"); } }

		string _outputTemplate = "{o} - Processed";
		public string OutputTemplate { get { return _outputTemplate; } set { _outputTemplate = value; PropChanged("OutputTemplate"); PropChanged("OutputTemplateExample"); } }
		public string OutputTemplateExample
		{
			get 
			{
				var str = OutputTemplate.Trim() + ".jpg";
				str = str.Replace("{o}", "DSCF3013");
				str = str.Replace("{w}", "1920");
				str = str.Replace("{h}", "1080");
				return str;
			}
		}

		#endregion

		#region Checkboxes

		bool _enableRotation;
		public bool EnableRotation { get { return _enableRotation; } set { _enableRotation = value; PropChanged("EnableRotation"); } }

		bool _enableCrop;
		public bool EnableCrop { get { return _enableCrop; } set { _enableCrop = value; PropChanged("EnableCrop"); } }

		bool _enableResize;
		public bool EnableResize { get { return _enableResize; } set { _enableResize = value; PropChanged("EnableResize"); } }

		bool _enableWatermark;
		public bool EnableWatermark { get { return _enableWatermark; } set { _enableWatermark = value; PropChanged("EnableWatermark"); } }

		#endregion

		#region Progress

		int _totalImages = 1;
		public int TotalImages
		{
			get
			{
				return _totalImages;
			}
			private set
			{
				_totalImages = value;
				PropChanged("TotalImages");
			}
		}

		int _doneImages = 1;
		public int DoneImages
		{
			get
			{
				return _doneImages;
			}
			private set
			{
				_doneImages = value;
				PropChanged("DoneImages");

				if (DoneImages == TotalImages)
					PropChanged("Ready");
			}
		}

		bool _showProgressBar;
		public bool ShowProgressBar { get { return _showProgressBar; } set { _showProgressBar = value; PropChanged("ShowProgressBar"); } }

		#endregion

		public bool Ready
		{
			get 
			{
				return OutputSet && (DoneImages == TotalImages);
			}
		}

		#endregion

		public ViewModel()
		{
			Folders = new ObservableCollection<Folder>();
			Engine.UpdateDone += Engine_UpdateDone;
		}

		#region Property Changed Stuff

		public void PropChanged(string val)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(val));
		}

		public event PropertyChangedEventHandler PropertyChanged; 

		#endregion

		#region File/Folder Removal Overloads

		internal void RemoveFolder(Folder folder)
		{
			foreach (var parent in Folders)
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

			return parent.Files.Cast<Folder>().Any(p => RemoveFolder(p, folder));
		}

		internal void RemoveFile(FileWrapper file)
		{
			RemoveFile(file, Folders[0]);
		}

		private bool RemoveFile(FileWrapper file, Folder folder)
		{
			if (folder.Files.Contains(file))
			{
				folder.Files.Remove(file);
				return true;
			}

			return folder.Files.Cast<Folder>().Any(p => RemoveFile(file, p));
		} 

		#endregion

		public void Dispose()
		{
			_watermarkFont.Dispose();
			GC.SuppressFinalize(this);
		}

		public void Begin()
		{
			ShowProgressBar = true;
			TotalImages = 1;
			DoneImages = 0;
			PropChanged("Ready");
			Engine.Process(this);
		}

		void Engine_UpdateDone()
		{
			TotalImages = Engine.TotalImages;
			DoneImages = Engine.DoneImages;

			if (DoneImages == TotalImages)
				ShowProgressBar = false;
		}
	}
}