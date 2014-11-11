using System.ComponentModel;
using System.Windows;

namespace BatchImageProcessor.View
{
	/// <summary>
	/// Interaction logic for RenameFileDialog.xaml
	/// </summary>
	public partial class RenameFileDialog : INotifyPropertyChanged
	{
		public RenameFileDialog()
		{
			InitializeComponent();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void cancelBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void okBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
