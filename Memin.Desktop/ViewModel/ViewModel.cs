// Copyright (c) 2017 Javier Cañon www.javiercanon.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//using BatchImageProcessor.Annotations;
using BatchImageProcessor.Model;
using BatchImageProcessor.Model.Annotations;
using BatchImageProcessor.Model.Interface;
using BatchImageProcessor.Model.Types;
using BatchImageProcessor.Model.Types.Enums;
//using BatchImageProcessor.Properties;
//using BatchImageProcessor.View;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
//using RawOptions = BatchImageProcessor.View.RawOptions;
using ResizeMode = BatchImageProcessor.Model.Types.Enums.ResizeMode;

namespace Memin.Desktop.ViewModel
{
    public class ViewModel : IDisposable, INotifyPropertyChanged, IProgress<ModelProgressUpdate>
    {
        //public Model.Model Model { get; }
        public Model Model { get; }

        public OptionSet OptionSet => Model.Options;

        public ObservableCollection<IFolderableHost> Folders => Model.Folders;

        private readonly IProgress<ModelProgressUpdate> _windowProgress;

        #region Ctor Dtor

        public ViewModel(IProgress<ModelProgressUpdate> windowProgress = null)
        {
            Model = new Model();

            OptionSet.OutputOptions.OutputTemplate = Resources.OutputTemplate;
            OptionSet.WatermarkOptions.WatermarkImagePath = Resources.NoFileSet;
            OptionSet.WatermarkOptions.WatermarkText = Resources.WatermarkText;

            OutputPath = Resources.ViewModel__outputPath__No_Path_Set;

            _windowProgress = windowProgress;
        }

        public void Dispose()
        {
            OptionSet.WatermarkOptions.WatermarkFont.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// RUN
        /// </summary>
        /// <returns></returns>
        public async Task Begin()
        {
            ShowProgressBar = true;
            TotalImages = 1;
            DoneImages = 0;
            PropChanged(nameof(Ready));

            await Model.Process(this);
            ShowProgressBar = false;
        }

        public void Cancel()
        {
            Model.Cancel = true;
        }

        #region Properties

        private static bool _paintDotNetSet;
        private static string _paintDotNetInstall;
        private static bool _paintDotNetInstalled;

        public bool Ready => OutputSet && (DoneImages == TotalImages);

        public static bool PaintDotNetInstalled
        {
            get
            {
                if (!_paintDotNetSet)
                    return PaintDotNetInstalled = IsPaintDotNetInstalled();
                return _paintDotNetInstalled;
            }
            private set
            {
                _paintDotNetInstalled = value;
            }
        }

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

        #region Message Settings

        public string MessageText
        {
            get { return OptionSet.MessageOptions.MessageText; }
            set
            {
                OptionSet.MessageOptions.MessageText = value;
                PropChanged();
            }
        }


        public Font MessageFont
        {
            get { return OptionSet.MessageOptions.MessageFont; }
            set
            {
                OptionSet.MessageOptions.MessageFont = value;
                PropChanged();
                PropChanged(nameof(MessageFontString));
            }
        }

        public int MessageColor
        {
            get { return OptionSet.MessageOptions.MessageColor; }
            set
            {
                OptionSet.MessageOptions.MessageColor = value;
                PropChanged();
            }
        }


        public string MessageFontString => OptionSet.MessageOptions.MessageFontString;


        public double MessageOpacity
        {
            get { return OptionSet.MessageOptions.MessageOpacity; }
            set
            {
                OptionSet.MessageOptions.MessageOpacity = value;
                PropChanged();
            }
        }

        public Alignment MessageAlignment
        {
            get { return OptionSet.MessageOptions.MessageAlignment; }
            set
            {
                OptionSet.MessageOptions.MessageAlignment = value;
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
                PropChanged(nameof(WatermarkFontString));
            }
        }

        public int WatermarkColor
        {
            get { return OptionSet.WatermarkOptions.WatermarkColor; }
            set
            {
                OptionSet.WatermarkOptions.WatermarkColor = value;
                PropChanged();
            }
        }


        public string WatermarkFontString => OptionSet.WatermarkOptions.WatermarkFontString;


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
            set
            {
                OptionSet.AdjustmentOptions.ColorType = value;
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
                PropChanged(nameof(Ready));
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
                PropChanged(nameof(OutputTemplateExample));
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
            get { return OptionSet.OutputOptions.OutputFormat; }
            set { OptionSet.OutputOptions.OutputFormat = value; PropChanged(); }
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


        public bool EnableMessage
        {
            get { return OptionSet.EnableMessage; }
            set
            {
                OptionSet.EnableMessage = value;
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
        private int _totalImages;
        private int _doneImages;

        public int TotalImages
        {
            get { return _totalImages; }
            private set
            {
                _totalImages = value;
                PropChanged();
            }
        }

        public int DoneImages
        {
            get { return _doneImages; }
            private set
            {
                _doneImages = value;
                PropChanged();

                if (DoneImages == TotalImages)
                    PropChanged(nameof(Ready));
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

        #region Misc Methods

        private static bool IsPaintDotNetInstalled()
        {
            try
            {
                var k = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).OpenSubKey("SOFTWARE");

                k = k?.OpenSubKey("paint.net");

                if (k == null) return false;

                _paintDotNetSet = true;
                _paintDotNetInstall = new DirectoryInfo((string)k.GetValue("TARGETDIR")).FullName + "\\PaintDotNet.exe";
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void OpenWithPaintDotNet(IFile file)
        {
            if (!PaintDotNetInstalled) throw new FileNotFoundException("Paint.NET is not installed.");

            Process.Start(_paintDotNetInstall, $"\"{file.Path}\"");
        }

        #endregion

        #region File/Folder Add/Removal Overloads


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
            RemoveFile(file, Model.Folders[0]);
        }

        private static bool RemoveFile(IFolderable file, IFolderableHost folderWrapper)
        {
            if (!folderWrapper.Files.Contains(file))
                return folderWrapper.Files.OfType<IFolderableHost>().Any(p => RemoveFile(file, p));
            folderWrapper.Files.Remove(file);
            return true;
        }


        public void ImportFiles(string[] filepaths, string[] msgText, FolderWrapper folder)
        {
            // BatchImageProcessor.Model.Types.RawOptions overrideOptions = null;

            bool bError = false;
            string sError = "", msg = "";

            int index = 0;

            foreach (var strfile in filepaths)
            {
                var f = new FileInfo(strfile);
                if (!f.Exists)
                {
                    bError = true;
                    sError = Resources.File_does_not_exist + ".\r\n" + strfile;
                    break; // Doesn't exist
                }

                //check for msg
                if (msgText != null)
                {

                    if (index < msgText.Length)
                    {
                        msg = msgText[index];  //it exists
                    }

                }

                if (msgText == null)
                {
                    folder.AddFile(strfile);
                }
                else
                {
                    this.MessageText = msg;
                    folder.AddFile(strfile, msg);
                }

                index++;
            }

            if (bError)
            {
                MessageBox.Show(sError, Resources.Import_Error);
            }

        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void PropChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IProgress<ModelProgressUpdate>

        public void Report(ModelProgressUpdate value)
        {
            TotalImages = value.Total;
            DoneImages = value.Done;
            _windowProgress?.Report(value);
        }

        #endregion
    }
}