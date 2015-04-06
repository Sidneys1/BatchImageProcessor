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
	    public Model.Model Model { get; }
		public OptionSet OptionSet => Model.Options;

	    public ObservableCollection<IFolderableHost> Folders => Model.Folders;

		public ViewModel()
		{
			Model = new Model.Model();
			OptionSet.OutputOptions.OutputTemplate = Resources.OutputTemplate;
			OptionSet.WatermarkOptions.WatermarkImagePath = Resources.NoFileSet;
			OptionSet.WatermarkOptions.WatermarkText = Resources.WatermarkText;

			OutputPath= Resources.ViewModel__outputPath__No_Path_Set;
			Model.UpdateDone += Engine_UpdateDone;
        }

        public void Dispose()
        {
			OptionSet.WatermarkOptions.WatermarkFont.Dispose();
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

	    public Rotation Rotation
        {
            get { return OptionSet.Rotation; }
            set
            {
                OptionSet.Rotation = value;
                PropChanged();
            }
        }

        #endregion

        #region Crop Settings

	    public Alignment CropAlignment
        {
            get { return OptionSet.CropOptions.CropAlignment; }
            set
            {
				OptionSet.CropOptions.CropAlignment = value;
                PropChanged();
            }
        }

        public int CropWidth
        {
            get { return OptionSet.CropOptions.CropWidth; }
            set
            {
                OptionSet.CropOptions.CropWidth = value;
                PropChanged();
            }
        }

        public int CropHeight
        {
            get { return OptionSet.CropOptions.CropHeight; }
            set
            {
                OptionSet.CropOptions.CropHeight = value;
                PropChanged();
            }
        }

        #endregion

        #region Resize Settings

	    public ResizeMode ResizeMode
        {
            get { return OptionSet.ResizeOptions.ResizeMode; }
            set
            {
                OptionSet.ResizeOptions.ResizeMode = value;
                PropChanged();
            }
        }

        public bool UseAspectRatio
        {
            get { return OptionSet.ResizeOptions.UseAspectRatio; }
            set
            {
                OptionSet.ResizeOptions.UseAspectRatio = value;
                PropChanged();
            }
        }

        public int ResizeWidth
        {
            get { return OptionSet.ResizeOptions.ResizeWidth; }
            set
            {
                OptionSet.ResizeOptions.ResizeWidth = value;
                PropChanged();
            }
        }

        public int ResizeHeight
        {
            get { return OptionSet.ResizeOptions.ResizeHeight; }
            set
            {
                OptionSet.ResizeOptions.ResizeHeight = value;
                PropChanged();
            }
        }

        #endregion

        #region Watermark Settings

	    public WatermarkType WatermarkType
        {
            get { return OptionSet.WatermarkOptions.WatermarkType; }
            set
            {
                OptionSet.WatermarkOptions.WatermarkType = value;
                PropChanged();
            }
        }

        public string WatermarkText
        {
            get { return OptionSet.WatermarkOptions.WatermarkText; }
            set
            {
                OptionSet.WatermarkOptions.WatermarkText = value;
                PropChanged();
            }
        }

        public Font WatermarkFont
        {
            get { return OptionSet.WatermarkOptions.WatermarkFont; }
            set
            {
                OptionSet.WatermarkOptions.WatermarkFont = value;
                PropChanged();
                PropChanged("WatermarkFontString");
            }
        }

        public string WatermarkFontString
            => string.Format("{0}, {1}pt", WatermarkFont.FontFamily.Name, WatermarkFont.SizeInPoints);

        public double WatermarkOpacity
        {
            get { return OptionSet.WatermarkOptions.WatermarkOpacity; }
            set
            {
                OptionSet.WatermarkOptions.WatermarkOpacity = value;
                PropChanged();
            }
        }

        public Alignment WatermarkAlignment
        {
            get { return OptionSet.WatermarkOptions.WatermarkAlignment; }
            set
            {
                OptionSet.WatermarkOptions.WatermarkAlignment = value;
                PropChanged();
            }
        }

        public string WatermarkImagePath
        {
            get { return OptionSet.WatermarkOptions.WatermarkImagePath; }
            set
            {
                OptionSet.WatermarkOptions.WatermarkImagePath = value;
                PropChanged();
            }
        }

        public bool WatermarkGreyscale
        {
            get { return OptionSet.WatermarkOptions.WatermarkGreyscale; }
            set
            {
                OptionSet.WatermarkOptions.WatermarkGreyscale = value;
                PropChanged();
            }
        }

		#endregion

		#region Color Settings

	    public ColorType ColorType
	    {
		    get { return OptionSet.AdjustmentOptions.ColorType; }
		    set { OptionSet.AdjustmentOptions.ColorType = value;
			    PropChanged();
		    }
	    }

		public double ColorBrightness
		{
			get { return OptionSet.AdjustmentOptions.ColorBrightness; }
			set
			{
				OptionSet.AdjustmentOptions.ColorBrightness = value;
				PropChanged();
			}
		}

		public double ColorContrast
		{
			get { return OptionSet.AdjustmentOptions.ColorContrast; }
			set
			{
				OptionSet.AdjustmentOptions.ColorContrast = value;
				PropChanged();
			}
		}

		public double ColorSaturation
		{
			get { return OptionSet.AdjustmentOptions.ColorSaturation; }
			set
			{
				OptionSet.AdjustmentOptions.ColorSaturation = value;
				PropChanged();
			}
		}

		public double ColorGamma
		{
			get { return OptionSet.AdjustmentOptions.ColorGamma; }
			set
			{
				OptionSet.AdjustmentOptions.ColorGamma = value;
				PropChanged();
			}
		}

		#endregion

		#region OutputSettings

	    public string OutputPath
        {
            get { return OptionSet.OutputOptions.OutputPath; }
            set
            {
                OptionSet.OutputOptions.OutputPath = value;
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
            get { return OptionSet.OutputOptions.NameOption; }
            set
            {
                OptionSet.OutputOptions.NameOption = value;
                PropChanged();
            }
        }

        public string OutputTemplate
        {
            get { return OptionSet.OutputOptions.OutputTemplate; }
            set
            {
                OptionSet.OutputOptions.OutputTemplate = value;
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
			get { return OptionSet.OutputOptions.OutputFormat;}
			set { OptionSet.OutputOptions.OutputFormat = value;PropChanged(); }
	    }

		public double JpegQuality
		{ get { return OptionSet.OutputOptions.JpegQuality; } set { OptionSet.OutputOptions.JpegQuality = value; PropChanged(); } }

		#endregion

		#region Checkboxes

	    public bool EnableRotation
        {
            get { return OptionSet.EnableRotation; }
            set
            {
                OptionSet.EnableRotation = value;
                PropChanged();
            }
        }

        public bool EnableCrop
        {
            get { return OptionSet.EnableCrop; }
            set
            {
                OptionSet.EnableCrop = value;
                PropChanged();
            }
        }

        public bool EnableResize
        {
            get { return OptionSet.EnableResize; }
            set
            {
                OptionSet.EnableResize = value;
                PropChanged();
            }
        }

        public bool EnableWatermark
        {
            get { return OptionSet.EnableWatermark; }
            set
            {
                OptionSet.EnableWatermark = value;
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