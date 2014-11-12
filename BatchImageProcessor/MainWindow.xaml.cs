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
using BatchImageProcessor.Model;
using BatchImageProcessor.View;
using BatchImageProcessor.ViewModel;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Control = System.Windows.Forms.Control;
using File = BatchImageProcessor.Model.File;
using IWin32Window = System.Windows.Forms.IWin32Window;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace BatchImageProcessor
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : IWin32Window
	{
		#region Properties/Variables

		#region Dialogs

		private readonly OpenFileDialog _fileBrowser = new OpenFileDialog
		{
			Title = Properties.Resources.MainWindow__fileBrowser_Title,
			CheckFileExists = true,
			CheckPathExists = true,
			Filter = Properties.Resources.MainWindow__fileBrowser_Filter,
			Multiselect = true,
			InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
		};

		private readonly FolderBrowserDialog _folderBrowser = new FolderBrowserDialog
		{
			Description = Properties.Resources.MainWindow_FolderBrowser_Description,
			RootFolder = Environment.SpecialFolder.MyComputer,
			ShowNewFolderButton = false
		};

		private readonly FolderBrowserDialog _outputBrowser = new FolderBrowserDialog
		{
			Description = Properties.Resources.MainWindow_OutputBrowser_Description,
			RootFolder = Environment.SpecialFolder.MyComputer,
			ShowNewFolderButton = true
		};

		private readonly OpenFileDialog _watermarkFileBrowser = new OpenFileDialog
		{
			Title = Properties.Resources.MainWindow__watermarkFileBrowser_Title,
			CheckFileExists = true,
			CheckPathExists = true,
			Filter = Properties.Resources.MainWindow__watermarkFileBrowser_Filter,
			Multiselect = false,
			InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
		};

		private readonly FontDialog _watermarkFontDlg = new FontDialog
		{
			ShowColor = false
		};

		#endregion

		private readonly IntPtr _handle;

		public ViewModel.ViewModel VModel { get; private set; }

		public IntPtr Handle
		{
			get { return _handle; }
		}

		#endregion

		public MainWindow()
		{
			VModel = new ViewModel.ViewModel();
			VModel.Folders.Add(new Folder(removable: false) {Name = "Output Folder"});

			InitializeComponent();

			//this.DataContext = vModel;

			_watermarkFontDlg.Font = VModel.WatermarkFont;

			_handle = new WindowInteropHelper(this).Handle;

#if !DEBUG
			gcBtn.Visibility = System.Windows.Visibility.Collapsed;
#endif
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var args = Environment.GetCommandLineArgs();

			if (args.Length > 1 && args.Contains("-noshaders"))
			{
				Resources["tdse"] = null;
			}
		}

		#region Thumbnail Grid

		#region Grid View Manipulation Buttons

		private void selectAllBtn_Click(object sender, RoutedEventArgs e)
		{
			listView.SelectAll();
		}

		private void deselectBtn_Click(object sender, RoutedEventArgs e)
		{
			listView.SelectedIndex = -1;
		}

		private void checkAllBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper f in listView.SelectedItems)
			{
				f.Selected = true;
			}
		}

		private void uncheckBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper f in listView.SelectedItems)
			{
				f.Selected = false;
			}
		}

		#endregion

		private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			var files = new List<FileInfo>();
			FileWrapper wrapper;
			if (sender is Grid)
			{
				files.AddRange(from File file in listView.SelectedItems select new FileInfo(file.Path));
				wrapper = listView.SelectedItem as FileWrapper;
			}
			else
			{
				var frameworkElement = e.Source as FrameworkElement;
				if (frameworkElement != null)
				{
					var f = frameworkElement.DataContext as File;
					if (f != null) files.Add(new FileInfo(f.Path));
				}
				wrapper = treeView.SelectedItem as FileWrapper;
			}

			if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				var scm = new ShellContextMenu();

				scm.ShowContextMenu(new WindowInteropHelper(this).Handle, files.ToArray(), Control.MousePosition);
			}
			else
			{
				var ctxMnu = Resources["imageCtxMenu"] as ContextMenu;
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
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Rotation.CounterClockwise;
			}
		}

		private void noRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Rotation.None;
			}
		}

		private void defRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Rotation.Default;
			}
		}

		private void upRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Rotation.UpsideDown;
			}
		}

		private void cRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Rotation.Clockwise;
			}
		}

		private void portRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Rotation.Portrait;
			}
		}

		private void landRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Rotation.Landscape;
			}
		}

		#endregion

		#region File/Folder Interaction Buttons

		private void importFolderBtn_Click(object sender, RoutedEventArgs e)
		{
			if (_folderBrowser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				var inf = new DirectoryInfo(_folderBrowser.SelectedPath);
				var recurs = false;
				if (inf.GetDirectories().Length > 0)
				{
					var res = MessageBox.Show(this,
						"This folder has folders within it. Would you like to add those as well?",
						"Import Folder", MessageBoxButton.YesNoCancel);
					if (res == MessageBoxResult.Cancel)
						return;
					if (res == MessageBoxResult.Yes)
						recurs = true;
				}

				var f = new Folder(_folderBrowser.SelectedPath, recurs);

				Folder parent;

				if (e.Source is MenuItem)
				{
					parent = (e.Source as MenuItem).DataContext as Folder;
				}
				else
				{
					if (treeView.SelectedItem is Folder)
						parent = (treeView.SelectedItem as Folder);
					else
						parent = VModel.Folders[0];
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

				if (parent != null) parent.Files.Add(f);
			}
		}

		private void RemoveFolderMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Folder parent;

			if (e.Source is MenuItem)
			{
				parent = (e.Source as MenuItem).DataContext as Folder;
			}
			else
			{
				if (treeView.SelectedItem is Folder)
					parent = (treeView.SelectedItem as Folder);
				else
					parent = VModel.Folders[0];
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

			if (e.Source is MenuItem)
			{
				parent = (e.Source as MenuItem).DataContext as Folder;
			}
			else
			{
				if (treeView.SelectedItem is Folder)
					parent = (treeView.SelectedItem as Folder);
				else
					parent = VModel.Folders[0];
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

			if (parent != null) parent.Files.Add(f);
		}

		private void importImageBtn_Click(object sender, RoutedEventArgs e)
		{
			if (_fileBrowser.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
			Folder parent;

			if (e.Source is MenuItem)
			{
				parent = (e.Source as MenuItem).DataContext as Folder;
			}
			else
			{
				if (treeView.SelectedItem is Folder)
					parent = (treeView.SelectedItem as Folder);
				else
					parent = VModel.Folders[0];
			}

			foreach (var str in _fileBrowser.FileNames.Where(str => parent != null))
			{
// ReSharper disable once PossibleNullReferenceException
				parent.Files.Add(new FileWrapper(str));
			}
		}

		private void RenameFolderMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var menuItem = e.Source as MenuItem;
			if (menuItem == null) return;
			var target = menuItem.DataContext as Folder;

			if (target == null) return;
			var oldName = target.Name;
			var fdlg = new RenameFileDialog {DataContext = target, Owner = this, Title = "Rename Folder"};

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
			var b = new AboutBox {Owner = this};
			b.ShowDialog();
		}

		#endregion

		#region Settings Handlers

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (RotateSettings == null) return;
			RotateSettings.Visibility =
				ResizeSettings.Visibility =
					CropSettings.Visibility = WatermarkSettings.Visibility = OutputSettings.Visibility = Visibility.Collapsed;
			if (SettingsPresenter == null) return;
			if (Equals(OptionsBox.SelectedItem, RotateBox))
				RotateSettings.Visibility = Visibility.Visible;
			else if (Equals(OptionsBox.SelectedItem, ResizeBox))
				ResizeSettings.Visibility = Visibility.Visible;
			else if (Equals(OptionsBox.SelectedItem, CropBox))
				CropSettings.Visibility = Visibility.Visible;
			else if (Equals(OptionsBox.SelectedItem, WatermarkBox))
				WatermarkSettings.Visibility = Visibility.Visible;
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
			if (Equals(sender, watermarkTlBtn))
				model.WatermarkAlignment = Alignment.Top_Left;
			else if (Equals(sender, watermarkTcBtn))
				model.WatermarkAlignment = Alignment.Top_Center;
			else if (Equals(sender, watermarkTrBtn))
				model.WatermarkAlignment = Alignment.Top_Right;

			else if (Equals(sender, watermarkMlBtn))
				model.WatermarkAlignment = Alignment.Middle_Left;
			else if (Equals(sender, watermarkMcButton))
				model.WatermarkAlignment = Alignment.Middle_Center;
			else if (Equals(sender, watermarkMrBtn))
				model.WatermarkAlignment = Alignment.Middle_Right;

			else if (Equals(sender, watermarkBlBtn))
				model.WatermarkAlignment = Alignment.Bottom_Left;
			else if (Equals(sender, watermarkBcBtn))
				model.WatermarkAlignment = Alignment.Bottom_Center;
			else if (Equals(sender, watermarkBrBtn))
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
			if (_outputBrowser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				VModel.OutputPath = _outputBrowser.SelectedPath;
				VModel.OutputSet = true;
			}
		}

		#endregion
	}
}