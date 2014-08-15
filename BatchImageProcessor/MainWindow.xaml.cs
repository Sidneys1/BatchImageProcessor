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

			//Model.Folder f = new Model.Folder(@"D:\Documents\New folder (2)");

			ViewModel.ViewModel context = new ViewModel.ViewModel();
			context.Folders.Add(new ViewModel.Folder(@"D:\Documents\New folder (2)\New folder"));
			this.DataContext = context;
		}

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
	}
}
