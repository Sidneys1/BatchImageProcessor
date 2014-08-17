using BatchImageProcessor.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace BatchImageProcessor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			ViewModel.ViewModel context = new ViewModel.ViewModel();
			context.Folders.Add(new ViewModel.Folder(@"D:\Documents\New folder (2)\New folder"));
			this.DataContext = context;
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

			if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				AndreasJohansson.Win32.Shell.ShellContextMenu scm = new AndreasJohansson.Win32.Shell.ShellContextMenu();

				scm.ShowContextMenu(new WindowInteropHelper(this).Handle, files.ToArray(), Control.MousePosition);
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
			RotateSettings.Visibility = ResizeSettings.Visibility = CropSettings.Visibility = WatermarkSettings.Visibility = OutputSettings.Visibility = Visibility.Collapsed;
			if (SettingsPresenter != null)
			{
				if (OptionsBox.SelectedItem == RotateBox)
					RotateSettings.Visibility = Visibility.Visible;
				else if (OptionsBox.SelectedItem == ResizeBox)
					ResizeSettings.Visibility = Visibility.Visible;
				else if (OptionsBox.SelectedItem == CropBox)
					CropSettings.Visibility = Visibility.Visible;
				else if (OptionsBox.SelectedItem == WatermarkBox)
					WatermarkSettings.Visibility = Visibility.Visible;
				else if (OptionsBox.SelectedItem == OutputBox)
					OutputSettings.Visibility = Visibility.Visible;
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
				model.DefaultCropAlignment = Alignment.Top_Left;
			else if (sender == cropBcBtn)
				model.DefaultCropAlignment = Alignment.Top_Center;
			else if (sender == cropBrBtn)
				model.DefaultCropAlignment = Alignment.Top_Right;
		}
	}
}
