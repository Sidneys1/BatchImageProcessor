using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using BatchImageProcessor.Annotations;
using BatchImageProcessor.Model.Interface;
using BatchImageProcessor.Model.Types;
using BatchImageProcessor.Model.Types.Enums;
using BatchImageProcessor.Properties;
using BatchImageProcessor.View;
using Microsoft.Win32;
using RawOptions = BatchImageProcessor.View.RawOptions;
using ResizeMode = BatchImageProcessor.Model.Types.Enums.ResizeMode;

namespace BatchImageProcessor.ViewModel
{
	public class ViewModel : IDisposable, INotifyPropertyChanged, IProgress<ModelProgressUpdate>
	{
		public Model.Model Model { get; }

		public OptionSet OptionSet => Model.Options;

		public ObservableCollection<IFolderableHost> Folders => Model.Folders;

		private readonly IProgress<ModelProgressUpdate> _windowProgress;

		#region Ctor Dtor

		public ViewModel(IProgress<ModelProgressUpdate> windowProgress = null)
		{
			Model = new Model.Model();
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

		public string WatermarkFontString
			=> $"{WatermarkFont.FontFamily.Name}, {WatermarkFont.SizeInPoints}pt";

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

		public void ImportFiles(MainWindow sender, string[] strs, FolderWrapper folder)
		{
			Model.Types.RawOptions overrideOptions = null;

			foreach (var str in strs)
			{
				var f = new FileInfo(str);
				if (!f.Exists)
				{
					MessageBox.Show(sender, $"File does not exist.\r\n{str}", "RAW Import Error");
					continue; // Doesn't exist
				}

				var rawExts = Resources.RawExts.Split('|')[1].Replace("*", "").Split(';');

				if (rawExts.Contains(f.Extension))
				{
					// RAW!
					var p = new Process
					{
						StartInfo = new ProcessStartInfo(".\\Exec\\dcraw.exe", $"-i \"{f.FullName}\"")
						{
							RedirectStandardOutput = true,
							UseShellExecute = false,
							CreateNoWindow = true
						}
					};

					p.Start();
					p.WaitForExit();

					if (p.ExitCode != 0)
					{
						MessageBox.Show(sender, $"Cannot decode RAW file.\r\n{str}", "RAW Import Error");
						continue; // Can't decode!
					}

					// RAW Import Options
					if (overrideOptions == null)
					{
						p.StartInfo.Arguments = $"-i -v \"{f.FullName}\"";
						p.Start();
						var info = p.StandardOutput.ReadToEnd();
						var x = new RawOptions(str, info) { Owner = sender };
						if (x.ShowDialog() == false) continue; // Cancelled!

						if (x.ApplyToAll)
							overrideOptions = (x.DataContext as FileWrapper)?.RawOptions;
						((FileWrapper)x.DataContext).IsRaw = true;
						folder.Files.Add(x.DataContext as FileWrapper);

						x.Close();
						continue;
					}
					folder.Files.Add(new FileWrapper(str) { RawOptions = overrideOptions.Copy(), IsRaw = true });
				}
				else
					folder.AddFile(str);
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