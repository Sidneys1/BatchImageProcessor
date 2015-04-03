using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using BatchImageProcessor.Model;
using BatchImageProcessor.View;
using BatchImageProcessor.ViewModel;
using Brushes = System.Windows.Media.Brushes;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Control = System.Windows.Forms.Control;
using Env = System.Environment;
using File = BatchImageProcessor.Model.File;
using IWin32Window = System.Windows.Forms.IWin32Window;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;

namespace BatchImageProcessor
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : IWin32Window, IDisposable
	{
		public MainWindow()
		{
			VModel = new ViewModel.ViewModel();
			VModel.Folders.Add(new Folder(removable: false) { Name = "Output Folder" });

			InitializeComponent();

			//this.DataContext = vModel;

			_watermarkFontDlg.Font = VModel.WatermarkFont;

			Handle = new WindowInteropHelper(this).Handle;

#if !DEBUG
			GcBtn.Visibility = Visibility.Collapsed;
#endif
		}

		private IntPtr _hwnd;
		private HwndSource _hsource;
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var args = Env.GetCommandLineArgs();

			if (args.Length <= 1 || !args.Contains("-noshaders")) return;
			Resources["DropShadowFx"] = null;
			Resources["BlurEffect"] = null;
		}


		#region Aero

		private void MainWindow_OnSourceInitialized(object sender, EventArgs e)
		{
			var args = Env.GetCommandLineArgs();
			if (args.Length > 1 && args.Contains("-noaero"))
				return;

			try
			{
				if ((_hwnd = new WindowInteropHelper(this).Handle) == IntPtr.Zero)
				{
					throw new InvalidOperationException("Could not get window handle for the main window.");
				}

				_hsource = HwndSource.FromHwnd(_hwnd);
				_hsource?.AddHook(WndProc);

				AdjustWindowFrame();
			}
			catch (InvalidOperationException)
			{
				FallbackPaint();
			}
		}

		private void AdjustWindowFrame()
		{
			if (DwmApiInterop.IsCompositionEnabled())
				ExtendFrameIntoClientArea(0, 0, (int)(RootGrid.ActualHeight - ContentRectangle.ActualHeight), 0);
			else
				FallbackPaint();
		}

		private void ExtendFrameIntoClientArea(int left, int right, int top, int bottom)
		{
			var margins = new Margins
			{
				cxLeftWidth = left,
				cxRightWidth = right,
				cyTopHeight = top,
				cyBottomHeight = bottom
			};
			var hresult = DwmApiInterop.ExtendFrameIntoClientArea(_hwnd, ref margins);

			if (hresult == 0)
			{
				if (_hsource.CompositionTarget != null) _hsource.CompositionTarget.BackgroundColor = Colors.Transparent;
				Background = Brushes.Transparent;
			}
			else
			{
				Trace.WriteLine("Could not extend window frames in the main window.");
			}
		}

		private void FallbackPaint()
		{
			Background = ContentRectangle.Fill;// Brushes.White;
		}

		private bool IsOnExtendedFrame(int lParam)
		{
			int x = lParam << 16 >> 16, y = lParam >> 16;
			var point = PointFromScreen(new Point(x, y));
			var result = VisualTreeHelper.HitTest(RootGrid, point);

			return result == null || (Equals(result.VisualHit, RootGrid));
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				// Ignore clicks if desktop composition isn't enabled
				case DwmApiInterop.WM_NCHITTEST:
					if (DwmApiInterop.IsCompositionEnabled()
						&& DwmApiInterop.IsOnClientArea(hwnd, msg, wParam, lParam)
						&& IsOnExtendedFrame(lParam.ToInt32()))
					{
						handled = true;
						return new IntPtr(DwmApiInterop.HTCAPTION);
					}

					return IntPtr.Zero;

				// Also toggle window frame painting on this window when desktop composition is toggled
				case DwmApiInterop.WM_DWMCOMPOSITIONCHANGED:
					try
					{
						AdjustWindowFrame();
					}
					catch (InvalidOperationException)
					{
						FallbackPaint();
					}
					return IntPtr.Zero;

				default:
					return IntPtr.Zero;
			}
		}

		#endregion

		#region Properties/Variables

		#region Dialogs

		private readonly OpenFileDialog _fileBrowser = new OpenFileDialog
		{
			Title = Properties.Resources.MainWindow__fileBrowser_Title,
			CheckFileExists = true,
			CheckPathExists = true,
			Filter = Properties.Resources.MainWindow__fileBrowser_Filter,
			Multiselect = true,
			InitialDirectory = Env.GetFolderPath(Env.SpecialFolder.MyPictures)
			
		};

		private readonly FolderBrowserDialog _folderBrowser = new FolderBrowserDialog
		{
			Description = Properties.Resources.MainWindow_FolderBrowser_Description,
			RootFolder = Env.SpecialFolder.MyComputer,
			ShowNewFolderButton = false
		};

		private readonly FolderBrowserDialog _outputBrowser = new FolderBrowserDialog
		{
			Description = Properties.Resources.MainWindow_OutputBrowser_Description,
			RootFolder = Env.SpecialFolder.MyComputer,
			SelectedPath = Env.GetFolderPath(Env.SpecialFolder.MyPictures),
			ShowNewFolderButton = true
		};

		private readonly OpenFileDialog _watermarkFileBrowser = new OpenFileDialog
		{
			Title = Properties.Resources.MainWindow__watermarkFileBrowser_Title,
			CheckFileExists = true,
			CheckPathExists = true,
			Filter = Properties.Resources.MainWindow__watermarkFileBrowser_Filter,
			Multiselect = false,
			InitialDirectory = Env.GetFolderPath(Env.SpecialFolder.MyPictures)
		};

		private readonly FontDialog _watermarkFontDlg = new FontDialog
		{
			ShowColor = false
		};

		#endregion

		public ViewModel.ViewModel VModel { get; }

		public IntPtr Handle { get; }

		#endregion

		#region Thumbnail Grid

		#region Grid View Manipulation Buttons

		private void selectAllBtn_Click(object sender, RoutedEventArgs e)
		{
			ThumbnailView.SelectAll();
		}

		private void deselectBtn_Click(object sender, RoutedEventArgs e)
		{
			ThumbnailView.SelectedIndex = -1;
		}

		private void checkAllBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper f in ThumbnailView.SelectedItems)
			{
				f.Selected = true;
			}
		}

		private void uncheckBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper f in ThumbnailView.SelectedItems)
			{
				f.Selected = false;
			}
		}

		private void SelectInvBtn_Click(object sender, RoutedEventArgs e)
		{
			var obj = new object[ThumbnailView.SelectedItems.Count];
			ThumbnailView.SelectedItems.CopyTo(obj, 0);
			ThumbnailView.SelectedIndex = -1;
			foreach (var item in ThumbnailView.Items.Cast<object>().Where(item => !obj.Contains(item)))
			{
				ThumbnailView.SelectedItems.Add(item);
			}

		}

		private void CheckInvBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper f in ThumbnailView.SelectedItems)
			{
				f.Selected = !f.Selected;
			}
		}

		#endregion

		private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			var files = new List<FileInfo>();
			FileWrapper wrapper;
			if (sender is Grid)
			{
				files.AddRange(from File file in ThumbnailView.SelectedItems select new FileInfo(file.Path));
				wrapper = ThumbnailView.SelectedItem as FileWrapper;
			}
			else
			{
				var frameworkElement = e.Source as FrameworkElement;
				var f = frameworkElement?.DataContext as File;
				if (f != null) files.Add(new FileInfo(f.Path));
				wrapper = TreeView.SelectedItem as FileWrapper;
			}

			if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				var scm = new ShellContextMenu();

				scm.ShowContextMenu(new WindowInteropHelper(this).Handle, files.ToArray(), Control.MousePosition);
			}
			else
			{
				//TryFindResource("ImageCtxMenu");
				var ctxMnu = Resources["ImageCtxMenu"] as ContextMenu;
				if (ctxMnu == null) return;
				ctxMnu.Placement = PlacementMode.Mouse;
				ctxMnu.DataContext = wrapper;
				ctxMnu.IsOpen = true;
			}
		}

		#endregion

		#region Rotation Button Handlers

		private void ccRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in ThumbnailView.SelectedItems)
			{
				item.RotationOverride = Rotation.CounterClockwise;
			}
		}

		private void noRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in ThumbnailView.SelectedItems)
			{
				item.RotationOverride = Rotation.None;
			}
		}

		private void defRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in ThumbnailView.SelectedItems)
			{
				item.RotationOverride = Rotation.Default;
			}
		}

		private void upRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in ThumbnailView.SelectedItems)
			{
				item.RotationOverride = Rotation.UpsideDown;
			}
		}

		private void cRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in ThumbnailView.SelectedItems)
			{
				item.RotationOverride = Rotation.Clockwise;
			}
		}

		private void portRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in ThumbnailView.SelectedItems)
			{
				item.RotationOverride = Rotation.Portrait;
			}
		}

		private void landRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in ThumbnailView.SelectedItems)
			{
				item.RotationOverride = Rotation.Landscape;
			}
		}

		#endregion

		#region File/Folder Interaction Buttons

		private void importFolderBtn_Click(object sender, RoutedEventArgs e)
		{
			if (_folderBrowser.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
			var inf = new DirectoryInfo(_folderBrowser.SelectedPath);
			var recurs = false;
			if (inf.GetDirectories().Length > 0)
			{
				var res = MessageBox.Show(this,
					"This folder has folders within it. Would you like to add those as well?",
					"Import Folder", MessageBoxButton.YesNoCancel);
				switch (res)
				{
					case MessageBoxResult.Cancel:
						return;
					case MessageBoxResult.Yes:
						recurs = true;
						break;
				}
			}

			var f = new Folder(_folderBrowser.SelectedPath, recurs);

			Folder parent;

			var item = e.Source as MenuItem;
			if (item != null)
			{
				parent = item.DataContext as Folder;
			}
			else
			{
				var folder = TreeView.SelectedItem as Folder;
				parent = folder ?? VModel.Folders[0];
			}

			if (parent != null && parent.ContainsFile(f.Name))
			{
				var s = f.Name + " ({0})";
				var i = 0;
				while (parent.ContainsFile(string.Format(s, ++i)))
				{
				}

				f.Name = string.Format(s, i);
			}

			parent?.Files.Add(f);
		}

		private void RemoveFolderMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Folder parent;

			var item = e.Source as MenuItem;
			if (item != null)
			{
				parent = item.DataContext as Folder;
			}
			else
			{
				var folder = TreeView.SelectedItem as Folder;
				parent = folder ?? VModel.Folders[0];
			}

			if (parent != null && parent.Removable)
				VModel.RemoveFolder(parent);
		}

		private void addFolderBtn_Click(object sender, RoutedEventArgs e)
		{
			var fdlg = new RenameFileDialog();
			var f = new Folder();
			fdlg.DataContext = f;
			fdlg.Owner = this;
			fdlg.Title = "Name New Folder";
			if (!fdlg.ShowDialog().GetValueOrDefault(false)) return;
			Folder parent;

			var item = e.Source as MenuItem;
			if (item != null)
			{
				parent = item.DataContext as Folder;
			}
			else
			{
				var folder = TreeView.SelectedItem as Folder;
				parent = folder ?? VModel.Folders[0];
			}

			f.Name = f.Name.Trim();

			if (parent != null && parent.ContainsFile(f.Name))
			{
				var s = f.Name + " ({0})";
				var i = 0;
				while (parent.ContainsFile(string.Format(s, ++i)))
				{
				}

				f.Name = string.Format(s, i);
			}

			parent?.Files.Add(f);
		}

		private void importImageBtn_Click(object sender, RoutedEventArgs e)
		{
			if (_fileBrowser.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
			Folder parent;

			var item = e.Source as MenuItem;
			if (item != null)
			{
				parent = item.DataContext as Folder;
			}
			else
			{
				var folder = TreeView.SelectedItem as Folder;
				parent = folder ?? VModel.Folders[0];
			}

			foreach (var str in _fileBrowser.FileNames)
			{
				parent?.Files.Add(new FileWrapper(str));
			}
		}

		private void RenameFolderMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var menuItem = e.Source as MenuItem;
			var target = menuItem?.DataContext as Folder;

			if (target == null) return;
			var oldName = target.Name;
			var fdlg = new RenameFileDialog { DataContext = target, Owner = this, Title = "Rename Folder" };

			if (fdlg.ShowDialog().GetValueOrDefault(false))
			{
				var parent = VModel.Folders[0];

				if (parent.ContainsFile(target.Name))
				{
					var s = target.Name + " ({0})";
					var i = 0;
					while (parent.ContainsFile(string.Format(s, ++i)))
					{
					}

					target.Name = string.Format(s, i);
				}

				target.Name = target.Name.Trim();
			}
			else
				target.Name = oldName;
		}

		#endregion

		#region Tool Buttons

		private void gcBtn_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Forcing GC");
			GC.Collect();
		}

		private void aboutBtn_Click(object sender, RoutedEventArgs e)
		{
			var b = new AboutBox { Owner = this };
			b.ShowDialog();
		}

		#endregion

		#region Settings Handlers

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (RotateSettings == null) return;
			RotateSettings.Visibility =
				ResizeSettings.Visibility =
					CropSettings.Visibility =
						WatermarkSettings.Visibility = 
							ColorSettings.Visibility =
								OutputSettings.Visibility = Visibility.Collapsed;
			if (SettingsPresenter == null) return;
			if (Equals(OptionsBox.SelectedItem, RotateBox))
				RotateSettings.Visibility = Visibility.Visible;
			else if (Equals(OptionsBox.SelectedItem, ResizeBox))
				ResizeSettings.Visibility = Visibility.Visible;
			else if (Equals(OptionsBox.SelectedItem, CropBox))
				CropSettings.Visibility = Visibility.Visible;
			else if (Equals(OptionsBox.SelectedItem, WatermarkBox))
				WatermarkSettings.Visibility = Visibility.Visible;
			else if (Equals(OptionsBox.SelectedItem, ColorBox))
				ColorSettings.Visibility = Visibility.Visible;
			else if (Equals(OptionsBox.SelectedItem, OutputBox))
				OutputSettings.Visibility = Visibility.Visible;

		}

		private void CropBtn_Click(object sender, RoutedEventArgs e)
		{
			var model = DataContext as ViewModel.ViewModel;

			if (model == null) return;

			if (Equals(sender, CropTopLeftBtn))
				model.DefaultCropAlignment = Alignment.Top_Left;
			else if (Equals(sender, CropTopCenterBtn))
				model.DefaultCropAlignment = Alignment.Top_Center;
			else if (Equals(sender, CropTopRightBtn))
				model.DefaultCropAlignment = Alignment.Top_Right;

			else if (Equals(sender, CropMiddleLeftBtn))
				model.DefaultCropAlignment = Alignment.Middle_Left;
			else if (Equals(sender, CropMiddleCenterButton))
				model.DefaultCropAlignment = Alignment.Middle_Center;
			else if (Equals(sender, CropMiddleRightBtn))
				model.DefaultCropAlignment = Alignment.Middle_Right;

			else if (Equals(sender, CropBottomLeftBtn))
				model.DefaultCropAlignment = Alignment.Bottom_Left;
			else if (Equals(sender, CropBottomCenterBtn))
				model.DefaultCropAlignment = Alignment.Bottom_Center;
			else if (Equals(sender, CropBottomRightBtn))
				model.DefaultCropAlignment = Alignment.Bottom_Right;
		}

		private void watermarkFontBtn_Click(object sender, RoutedEventArgs e)
		{
			var model = DataContext as ViewModel.ViewModel;
			if (model == null) return;
			_watermarkFontDlg.Font = model.WatermarkFont;
			if (_watermarkFontDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				model.WatermarkFont = _watermarkFontDlg.Font;
		}

		private void WatermarkBtn_Click(object sender, RoutedEventArgs e)
		{
			var model = DataContext as ViewModel.ViewModel;
			if (model == null) return;
			if (Equals(sender, WatermarkTopLeftBtn))
				model.WatermarkAlignment = Alignment.Top_Left;
			else if (Equals(sender, WatermarkTopCenterBtn))
				model.WatermarkAlignment = Alignment.Top_Center;
			else if (Equals(sender, WatermarkTopRightBtn))
				model.WatermarkAlignment = Alignment.Top_Right;

			else if (Equals(sender, WatermarkMiddleLeftBtn))
				model.WatermarkAlignment = Alignment.Middle_Left;
			else if (Equals(sender, WatermarkMiddleCenterButton))
				model.WatermarkAlignment = Alignment.Middle_Center;
			else if (Equals(sender, WatermarkMiddleRightBtn))
				model.WatermarkAlignment = Alignment.Middle_Right;

			else if (Equals(sender, WatermarkBottopLeftBtn))
				model.WatermarkAlignment = Alignment.Bottom_Left;
			else if (Equals(sender, WatermarkBottomCenterBtn))
				model.WatermarkAlignment = Alignment.Bottom_Center;
			else if (Equals(sender, WatermarkBottomRightBtn))
				model.WatermarkAlignment = Alignment.Bottom_Right;
		}

		private void watermarkImageBtn_Click(object sender, RoutedEventArgs e)
		{
			if (_watermarkFileBrowser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				VModel.WatermarkImagePath = _watermarkFileBrowser.FileName;
			}
		}

		private void outputBtn_Click(object sender, RoutedEventArgs e)
		{
			if (_outputBrowser.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
			VModel.OutputPath = _outputBrowser.SelectedPath;
			VModel.OutputSet = true;
		}

		#endregion

		#region IDisposable Support

		private bool _disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue) return;
			if (disposing)
			{
				_watermarkFontDlg.Dispose();
				_watermarkFileBrowser.Dispose();
				_outputBrowser.Dispose();
				_folderBrowser.Dispose();
				_fileBrowser.Dispose();
			}

			_disposedValue = true;
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}


		#endregion

		private void StartBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!VModel.Ready) return;

			VModel.Begin();
		}

		private void StopBtn_Click(object sender, RoutedEventArgs e)
		{
			Engine.Cancel = true;
		}


	}
}