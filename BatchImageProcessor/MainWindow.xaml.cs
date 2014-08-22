using BatchImageProcessor.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Linq;

namespace BatchImageProcessor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, System.Windows.Forms.IWin32Window
	{
		FolderBrowserDialog FolderBrowser = new FolderBrowserDialog() { Description = "Select a Folder to Import", RootFolder = System.Environment.SpecialFolder.MyComputer, ShowNewFolderButton = false };
		OpenFileDialog FileBrowser = new OpenFileDialog() 
		{
			Title = "Import Files...",
			CheckFileExists = true, 
			CheckPathExists = true, 
			Filter = "Image Files|*.jpg;*.jpeg",
			Multiselect = true,
			InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures)
		};

		public MainWindow()
		{
			InitializeComponent();

			ViewModel.ViewModel context = new ViewModel.ViewModel();
			context.Folders.Add(new Folder(removable: false) { Name = "Output Folder" });

			this.DataContext = context;

			_hnadle = new WindowInteropHelper(this).Handle;

#if !DEBUG
			gcBtn.Visibility = System.Windows.Visibility.Collapsed;
#endif
		}


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
			foreach (ViewModel.FileWrapper f in listView.SelectedItems)
			{
				f.Selected = true;
			}
		}

		private void uncheckBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (ViewModel.FileWrapper f in listView.SelectedItems)
			{
				f.Selected = false;
			}
		} 

		#endregion

		private void Grid_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			List<FileInfo> files = new List<FileInfo>();
			FileWrapper wrapper = null;
			if (sender is System.Windows.Controls.Grid)
			{
				foreach (Model.File file in listView.SelectedItems)
				{
					files.Add(new FileInfo(file.Path));
				}
				wrapper = listView.SelectedItem as FileWrapper;
			}
			else
			{
				Model.File f = (e.Source as FrameworkElement).DataContext as Model.File;
				files.Add(new FileInfo(f.Path));
				wrapper = treeView.SelectedItem as FileWrapper;
			}

			if ((System.Windows.Forms.Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				AndreasJohansson.Win32.Shell.ShellContextMenu scm = new AndreasJohansson.Win32.Shell.ShellContextMenu();

				scm.ShowContextMenu(new WindowInteropHelper(this).Handle, files.ToArray(), System.Windows.Forms.Control.MousePosition);
			}
			else
			{
				System.Windows.Controls.ContextMenu ctxMnu = this.Resources["imageCtxMenu"] as System.Windows.Controls.ContextMenu;
				ctxMnu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
				ctxMnu.DataContext = wrapper;
				ctxMnu.IsOpen = true;
				
			}
		}

		private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (RotateSettings != null)
			{
				RotateSettings.Visibility /*= ResizeSettings.Visibility*/ = CropSettings.Visibility /*= WatermarkSettings.Visibility = OutputSettings.Visibility*/ = Visibility.Collapsed;
				if (SettingsPresenter != null)
				{
					if (OptionsBox.SelectedItem == RotateBox)
						RotateSettings.Visibility = Visibility.Visible;
					//else if (OptionsBox.SelectedItem == ResizeBox)
					//	ResizeSettings.Visibility = Visibility.Visible;
					else if (OptionsBox.SelectedItem == CropBox)
						CropSettings.Visibility = Visibility.Visible;
					//else if (OptionsBox.SelectedItem == WatermarkBox)
					//	WatermarkSettings.Visibility = Visibility.Visible;
					//else if (OptionsBox.SelectedItem == OutputBox)
					//	OutputSettings.Visibility = Visibility.Visible;
				}
			}
		}

		#region Rotation Button Handlers

		private void ccRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Model.Rotation.CounterClockwise;
			}
		}

		private void noRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Model.Rotation.None;
			}
		}

		private void defRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Model.Rotation.Default;
			}
		}

		private void upRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Model.Rotation.UpsideDown;
			}
		}

		private void cRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Model.Rotation.Clockwise;
			}
		}

		private void portRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Model.Rotation.Portrait;
			}
		}

		private void landRotBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (FileWrapper item in listView.SelectedItems)
			{
				item.RotationOverride = Model.Rotation.Landscape;
			}
		}

		#endregion

		private void CropBtn_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.ViewModel model = this.DataContext as ViewModel.ViewModel;

			if (sender == cropTlBtn)
				model.DefaultCropAlignment = Alignment.Top_Left;
			else if (sender == cropTcBtn)
				model.DefaultCropAlignment = Alignment.Top_Center;
			else if (sender == cropTrBtn)
				model.DefaultCropAlignment = Alignment.Top_Right;

			else if (sender == cropMlBtn)
				model.DefaultCropAlignment = Alignment.Middle_Left;
			else if (sender == cropMcButton)
				model.DefaultCropAlignment = Alignment.Middle_Center;
			else if (sender == cropMrBtn)
				model.DefaultCropAlignment = Alignment.Middle_Right;

			else if (sender == CropBlBtn)
				model.DefaultCropAlignment = Alignment.Bottom_Left;
			else if (sender == cropBcBtn)
				model.DefaultCropAlignment = Alignment.Bottom_Center;
			else if (sender == cropBrBtn)
				model.DefaultCropAlignment = Alignment.Bottom_Right;
		}

		private void importFolderBtn_Click(object sender, RoutedEventArgs e)
		{
			if (FolderBrowser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				DirectoryInfo inf = new DirectoryInfo(FolderBrowser.SelectedPath);
				bool recurs = false;
				if (inf.GetDirectories().Length > 0)
				{
					MessageBoxResult res = System.Windows.MessageBox.Show(this, "This folder has folders within it. Would you like to add those as well?", "Import Folder", MessageBoxButton.YesNoCancel);
					if (res == MessageBoxResult.Cancel)
						return;
					else if (res == MessageBoxResult.Yes)
						recurs = true;
				}

				ViewModel.Folder f = new Folder(FolderBrowser.SelectedPath,recurs);

				Folder parent = null;

				if (e.Source is System.Windows.Controls.MenuItem)
				{
					parent = (e.Source as System.Windows.Controls.MenuItem).DataContext as Folder;
				}
				else
				{

					if (treeView.SelectedItem is Folder)
						parent = (treeView.SelectedItem as Folder);
					else
						parent = (this.DataContext as ViewModel.ViewModel).Folders[0];
				}

				if (parent.ContainsFile(f.Name))
				{
					string s = f.Name + " ({0})";
					int i = 0;
					while (parent.ContainsFile(string.Format(s, ++i))) ;

					f.Name = string.Format(s, i);
				}

				parent.Files.Add(f);
			}
		}

		System.IntPtr _hnadle;
		public System.IntPtr Handle
		{
			get { return _hnadle; }
		}

		private void RemoveFolderMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Folder parent = null;

			if (e.Source is System.Windows.Controls.MenuItem)
			{
				parent = (e.Source as System.Windows.Controls.MenuItem).DataContext as Folder;
			}
			else
			{

				if (treeView.SelectedItem is Folder)
					parent = (treeView.SelectedItem as Folder);
				else
					parent = (this.DataContext as ViewModel.ViewModel).Folders[0];
			}

			if (parent.Removable)
				(this.DataContext as ViewModel.ViewModel).RemoveFolder(parent);
		}

		private void addFolderBtn_Click(object sender, RoutedEventArgs e)
		{
			View.RenameFileDialog fdlg = new View.RenameFileDialog();
			Folder f = new Folder();
			fdlg.DataContext = f;
			fdlg.Owner = this;
			fdlg.Title = "Name New Folder";
			if (fdlg.ShowDialog().GetValueOrDefault(false))
			{
				Folder parent = null;

				if (e.Source is System.Windows.Controls.MenuItem)
				{
					parent = (e.Source as System.Windows.Controls.MenuItem).DataContext as Folder;
				}
				else
				{

					if (treeView.SelectedItem is Folder)
						parent = (treeView.SelectedItem as Folder);
					else
						parent = (this.DataContext as ViewModel.ViewModel).Folders[0];
				}

				f.Name = f.Name.Trim();

				if (parent.ContainsFile(f.Name))
				{
					string s = f.Name + " ({0})";
					int i = 0;
					while (parent.ContainsFile(string.Format(s, ++i))) ;

					f.Name = string.Format(s, i);
				}

				parent.Files.Add(f);
			}
		}

		private void importImageBtn_Click(object sender, RoutedEventArgs e)
		{
			if (FileBrowser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				Folder parent = null;

				if (e.Source is System.Windows.Controls.MenuItem)
				{
					parent = (e.Source as System.Windows.Controls.MenuItem).DataContext as Folder;
				}
				else
				{

					if (treeView.SelectedItem is Folder)
						parent = (treeView.SelectedItem as Folder);
					else
						parent = (this.DataContext as ViewModel.ViewModel).Folders[0];
				}

				foreach (string str in FileBrowser.FileNames)
				{
					parent.Files.Add(new FileWrapper(str));
				}
			}
		}

		private void gcBtn_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Forcing GC");
			System.GC.Collect();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			string[] args = Environment.GetCommandLineArgs();

			if (args.Length>1 && args.Contains("-noshaders"))
			{
				Resources["tdse"] = null;
			}
		}

		private void aboutBtn_Click(object sender, RoutedEventArgs e)
		{
			AboutBox b = new AboutBox();
			b.Owner = this;
			b.ShowDialog();
		}
	}
}
