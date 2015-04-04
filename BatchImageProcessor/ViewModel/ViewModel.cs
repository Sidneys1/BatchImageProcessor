using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using BatchImageProcessor.Annotations;
using BatchImageProcessor.Interface;
using BatchImageProcessor.Properties;
using BatchImageProcessor.Types;

namespace BatchImageProcessor.ViewModel
{
    public class ViewModel :IDisposable, INotifyPropertyChanged
    {
	    public Model.Model Model;

	    public ObservableCollection<IFolderableHost> Folders => Model.Folders;

		public ViewModel()
        {
	        Model = new Model.Model
	        {
		        OutputTemplate = Resources.OutputTemplate,
				WatermarkImagePath = Resources.NoFileSet,
				WatermarkText = Resources.WatermarkText
			};
			OutputPath= Resources.ViewModel__outputPath__No_Path_Set;
			Model.UpdateDone += Engine_UpdateDone;
        }

        public void Dispose()
        {
            Model.WatermarkFont.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Begin()
        {
            ShowProgressBar = true;
            TotalImages = 1;
            DoneImages = 0;
            PropChanged("Ready");
			Model.Process();
        }

        private void Engine_UpdateDone(object sender, EventArgs e)
        {
            if (DoneImages != TotalImages) return;
            ShowProgressBar = false;
			Model.Cancel = false;
        }

        #region Properties

	    public bool Ready => OutputSet && (DoneImages == TotalImages);

        #region Rotate Settings

	    public Rotation DefaultRotation
        {
            get { return Model.DefaultRotation; }
            set
            {
                Model.DefaultRotation = value;
                PropChanged();
            }
        }

        #endregion

        #region Crop Settings

	    public Alignment DefaultCropAlignment
        {
            get { return Model.DefaultCropAlignment; }
            set
            {
				Model.DefaultCropAlignment = value;
                PropChanged();
            }
        }

        public int CropWidth
        {
            get { return Model.CropWidth; }
            set
            {
                Model.CropWidth = value;
                PropChanged();
            }
        }

        public int CropHeight
        {
            get { return Model.CropHeight; }
            set
            {
                Model.CropHeight = value;
                PropChanged();
            }
        }

        #endregion

        #region Resize Settings

	    public ResizeMode DefaultResizeMode
        {
            get { return Model.DefaultResizeMode; }
            set
            {
                Model.DefaultResizeMode = value;
                PropChanged();
            }
        }

        public bool UseAspectRatio
        {
            get { return Model.UseAspectRatio; }
            set
            {
                Model.UseAspectRatio = value;
                PropChanged();
            }
        }

        public int ResizeWidth
        {
            get { return Model.ResizeWidth; }
            set
            {
                Model.ResizeWidth = value;
                PropChanged();
            }
        }

        public int ResizeHeight
        {
            get { return Model.ResizeHeight; }
            set
            {
                Model.ResizeHeight = value;
                PropChanged();
            }
        }

        #endregion

        #region Watermark Settings

	    public WatermarkType DefaultWatermarkType
        {
            get { return Model.DefaultWatermarkType; }
            set
            {
                Model.DefaultWatermarkType = value;
                PropChanged();
            }
        }

        public string WatermarkText
        {
            get { return Model.WatermarkText; }
            set
            {
                Model.WatermarkText = value;
                PropChanged();
            }
        }

        public Font WatermarkFont
        {
            get { return Model.WatermarkFont; }
            set
            {
                Model.WatermarkFont = value;
                PropChanged();
                PropChanged("WatermarkFontString");
            }
        }

        public string WatermarkFontString
            => string.Format("{0}, {1}pt", WatermarkFont.FontFamily.Name, WatermarkFont.SizeInPoints);

        public double WatermarkOpacity
        {
            get { return Model.WatermarkOpacity; }
            set
            {
                Model.WatermarkOpacity = value;
                PropChanged();
            }
        }

        public Alignment WatermarkAlignment
        {
            get { return Model.WatermarkAlignment; }
            set
            {
                Model.WatermarkAlignment = value;
                PropChanged();
            }
        }

        public string WatermarkImagePath
        {
            get { return Model.WatermarkImagePath; }
            set
            {
                Model.WatermarkImagePath = value;
                PropChanged();
            }
        }

        public bool WatermarkGreyscale
        {
            get { return Model.WatermarkGreyscale; }
            set
            {
                Model.WatermarkGreyscale = value;
                PropChanged();
            }
        }

		#endregion

		#region Color Settings

	    public ColorType ColorType
	    {
		    get { return Model.ColorType; }
		    set { Model.ColorType = value;
			    PropChanged();
		    }
	    }

		public double ColorBrightness
		{
			get { return Model.ColorBrightness; }
			set
			{
				Model.ColorBrightness = value;
				PropChanged();
			}
		}

		public double ColorContrast
		{
			get { return Model.ColorContrast; }
			set
			{
				Model.ColorContrast = value;
				PropChanged();
			}
		}

		public double ColorSaturation
		{
			get { return Model.ColorSaturation; }
			set
			{
				Model.ColorSaturation = value;
				PropChanged();
			}
		}

		public double ColorGamma
		{
			get { return Model.ColorGamma; }
			set
			{
				Model.ColorGamma = value;
				PropChanged();
			}
		}

		#endregion

		#region OutputSettings

	    public string OutputPath
        {
            get { return Model.OutputPath; }
            set
            {
                Model.OutputPath = value;
                PropChanged();
            }
        }

        public bool OutputSet
        {
            get { return Model.OutputSet; }
            set
            {
                Model.OutputSet = value;
                PropChanged();
                PropChanged("Ready");
            }
        }

        public NameType NameOption
        {
            get { return Model.NameOption; }
            set
            {
                Model.NameOption = value;
                PropChanged();
            }
        }

        public string OutputTemplate
        {
            get { return Model.OutputTemplate; }
            set
            {
                Model.OutputTemplate = value;
                PropChanged();
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

	    public Format OutputFormat
	    {
			get { return Model.OutputFormat;}
			set { Model.OutputFormat = value;PropChanged(); }
	    }

		public double JpegQuality
		{ get { return Model.JpegQuality; } set { Model.JpegQuality = value; PropChanged(); } }

		#endregion

		#region Checkboxes

	    public bool EnableRotation
        {
            get { return Model.EnableRotation; }
            set
            {
                Model.EnableRotation = value;
                PropChanged();
            }
        }

        public bool EnableCrop
        {
            get { return Model.EnableCrop; }
            set
            {
                Model.EnableCrop = value;
                PropChanged();
            }
        }

        public bool EnableResize
        {
            get { return Model.EnableResize; }
            set
            {
                Model.EnableResize = value;
                PropChanged();
            }
        }

        public bool EnableWatermark
        {
            get { return Model.EnableWatermark; }
            set
            {
                Model.EnableWatermark = value;
                PropChanged();
            }
        }

		#endregion

		#region Progress
		
        private bool _showProgressBar;
	    
	    public int TotalImages
        {
            get { return Model.TotalImages; }
            private set
            {
                Model.TotalImages = value;
                PropChanged();
            }
        }

        public int DoneImages
        {
            get { return Model.DoneImages; }
            private set
            {
                Model.DoneImages = value;
                PropChanged();

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
                PropChanged();
            }
        }

        #endregion

        #endregion
		
        #region File/Folder Removal Overloads

        internal void RemoveFolder(FolderWrapper folderWrapper)
        {
            foreach (var parent in Model.Folders.Cast<FolderWrapper>())
            {
                if (parent == folderWrapper)
                {
					Model.Folders.Remove(folderWrapper);
                    break;
                }

                if (RemoveFolder(parent, folderWrapper))
                    break;
            }
        }

        private static bool RemoveFolder(IFolderableHost parent, IFolderable folder)
        {
	        if (!parent.Files.Contains(folder)) return parent.Files.Cast<IFolderableHost>().Any(p => RemoveFolder(p, folder));
	        parent.Files.Remove(folder);

	        return true;
        }

        internal void RemoveFile(FileWrapper file)
        {
            RemoveFile(file, (FolderWrapper)Model.Folders[0]);
        }

        private static bool RemoveFile(IFolderable file, IFolderableHost folderWrapper)
        {
	        if (!folderWrapper.Files.Contains(file))
				return folderWrapper.Files.OfType<IFolderableHost>().Any(p => RemoveFile(file, p));
	        folderWrapper.Files.Remove(file);
	        return true;
        }

        #endregion

	    public void Cancel()
	    {
		    Model.Cancel = true;
	    }

	    public event PropertyChangedEventHandler PropertyChanged;

	    [NotifyPropertyChangedInvocator]
	    protected virtual void PropChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
    }
}