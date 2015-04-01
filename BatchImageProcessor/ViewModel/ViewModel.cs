using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using BatchImageProcessor.Model;
using Env= System.Environment;

namespace BatchImageProcessor.ViewModel
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public ViewModel()
        {
            Folders = new ObservableCollection<Folder>();
            Engine.UpdateDone += Engine_UpdateDone;
        }

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

        private void Engine_UpdateDone(object sender, EventArgs e)
        {
            TotalImages = Engine.TotalImages;
            DoneImages = Engine.DoneImages;

            if (DoneImages != TotalImages) return;
            ShowProgressBar = false;
            Engine.Cancel = false;
        }

        #region Properties

        public ObservableCollection<Folder> Folders { get; }

        public bool Ready => OutputSet && (DoneImages == TotalImages);

        #region Rotate Settings

        private Rotation _defaultRotation = Rotation.None;

        public Rotation DefaultRotation
        {
            get { return _defaultRotation; }
            set
            {
                _defaultRotation = value;
                PropChanged("DefaultRotation");
            }
        }

        #endregion

        #region Crop Settings

        private int _cropHeight = 600;
        private int _cropWidth = 800;
        private Alignment _defaultCropAlignment = Alignment.Middle_Center;

        public Alignment DefaultCropAlignment
        {
            get { return _defaultCropAlignment; }
            set
            {
                _defaultCropAlignment = value;
                PropChanged("DefaultCropAlignment");
            }
        }

        public int CropWidth
        {
            get { return _cropWidth; }
            set
            {
                _cropWidth = value;
                PropChanged("CropWidth");
            }
        }

        public int CropHeight
        {
            get { return _cropHeight; }
            set
            {
                _cropHeight = value;
                PropChanged("CropHeight");
            }
        }

        #endregion

        #region Resize Settings

        private ResizeMode _defaultResizeMode = ResizeMode.Smaller;
        private int _resizeHeight = 600;
        private int _resizeWidth = 800;

        private bool _useAspectRatio = true;

        public ResizeMode DefaultResizeMode
        {
            get { return _defaultResizeMode; }
            set
            {
                _defaultResizeMode = value;
                PropChanged("DefaultResizeMode");
            }
        }

        public bool UseAspectRatio
        {
            get { return _useAspectRatio; }
            set
            {
                _useAspectRatio = value;
                PropChanged("UseAspectRatio");
            }
        }

        public int ResizeWidth
        {
            get { return _resizeWidth; }
            set
            {
                _resizeWidth = value;
                PropChanged("ResizeWidth");
            }
        }

        public int ResizeHeight
        {
            get { return _resizeHeight; }
            set
            {
                _resizeHeight = value;
                PropChanged("ResizeHeight");
            }
        }

        #endregion

        #region Watermark Settings

        private WatermarkType _defaultWatermarkType = WatermarkType.Text;
        private Alignment _watermarkAlignment = Alignment.Bottom_Right;
        private Font _watermarkFont = new Font("Calibri", 12f);
        private bool _watermarkGreyscale = true;
        private string _watermarkImagePath = "<No File Set>";
        private double _watermarkOpacity = 0.7;

        private string _watermarkText = "Watermark Text";

        public WatermarkType DefaultWatermarkType
        {
            get { return _defaultWatermarkType; }
            set
            {
                _defaultWatermarkType = value;
                PropChanged("DefaultWatermarkType");
            }
        }

        public string WatermarkText
        {
            get { return _watermarkText; }
            set
            {
                _watermarkText = value;
                PropChanged("WatermarkText");
            }
        }

        public Font WatermarkFont
        {
            get { return _watermarkFont; }
            set
            {
                _watermarkFont = value;
                PropChanged("WatermarkFont");
                PropChanged("WatermarkFontString");
            }
        }

        public string WatermarkFontString
            => string.Format("{0}, {1}pt", WatermarkFont.FontFamily.Name, WatermarkFont.SizeInPoints);

        public double WatermarkOpacity
        {
            get { return _watermarkOpacity; }
            set
            {
                _watermarkOpacity = value;
                PropChanged("WatermarkOpacity");
            }
        }

        public Alignment WatermarkAlignment
        {
            get { return _watermarkAlignment; }
            set
            {
                _watermarkAlignment = value;
                PropChanged("WatermarkAlignment");
            }
        }

        public string WatermarkImagePath
        {
            get { return _watermarkImagePath; }
            set
            {
                _watermarkImagePath = value;
                PropChanged("WatermarkImagePath");
            }
        }

        public bool WatermarkGreyscale
        {
            get { return _watermarkGreyscale; }
            set
            {
                _watermarkGreyscale = value;
                PropChanged("WatermarkGreyscale");
            }
        }

		#endregion

		#region Color Settings

		

		#endregion

		#region OutputSettings

		private NameType _nameOption = NameType.Original;
        // TODO: switch back
        private string _outputPath = Env.GetFolderPath(Env.SpecialFolder.MyPictures); //Resources.ViewModel__outputPath__No_Path_Set;

        private bool _outputSet;
        private string _outputTemplate = "{o} - Processed";

        public string OutputPath
        {
            get { return _outputPath; }
            set
            {
                _outputPath = value;
                PropChanged("OutputPath");
            }
        }

        public bool OutputSet
        {
            get { return _outputSet; }
            set
            {
                _outputSet = value;
                PropChanged("OutputSet");
                PropChanged("Ready");
            }
        }

        public NameType NameOption
        {
            get { return _nameOption; }
            set
            {
                _nameOption = value;
                PropChanged("NameOption");
            }
        }

        public string OutputTemplate
        {
            get { return _outputTemplate; }
            set
            {
                _outputTemplate = value;
                PropChanged("OutputTemplate");
                PropChanged("OutputTemplateExample");
            }
        }

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

        private bool _enableCrop;
        private bool _enableResize;
        private bool _enableRotation;
        private bool _enableWatermark;
		private bool _enableColor;

		public bool EnableRotation
        {
            get { return _enableRotation; }
            set
            {
                _enableRotation = value;
                PropChanged("EnableRotation");
            }
        }

        public bool EnableCrop
        {
            get { return _enableCrop; }
            set
            {
                _enableCrop = value;
                PropChanged("EnableCrop");
            }
        }

        public bool EnableResize
        {
            get { return _enableResize; }
            set
            {
                _enableResize = value;
                PropChanged("EnableResize");
            }
        }

        public bool EnableWatermark
        {
            get { return _enableWatermark; }
            set
            {
                _enableWatermark = value;
                PropChanged("EnableWatermark");
            }
        }

		public bool EnableColor
		{
			get { return _enableColor; }
			set
			{
				_enableColor = value;
				PropChanged("EnableColor");
			}
		}

		#endregion

		#region Progress

		private int _doneImages;
        private bool _showProgressBar;
        private int _totalImages;

        public int TotalImages
        {
            get { return _totalImages; }
            private set
            {
                _totalImages = value;
                PropChanged("TotalImages");
            }
        }

        public int DoneImages
        {
            get { return _doneImages; }
            private set
            {
                _doneImages = value;
                PropChanged("DoneImages");

                if (DoneImages == TotalImages)
                    PropChanged("Ready");
            }
        }

        public bool ShowProgressBar
        {
            get { return _showProgressBar; }
            set
            {
                _showProgressBar = value;
                PropChanged("ShowProgressBar");
            }
        }

        #endregion

        #endregion

        #region Property Changed Stuff

        public event PropertyChangedEventHandler PropertyChanged;

        public void PropChanged(string val)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(val));
        }

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
    }
}