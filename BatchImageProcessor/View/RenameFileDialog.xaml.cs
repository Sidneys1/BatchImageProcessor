using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatchImageProcessor.View
{
	/// <summary>
	/// Interaction logic for RenameFileDialog.xaml
	/// </summary>
	public partial class RenameFileDialog : Window, INotifyPropertyChanged
	{
		public RenameFileDialog()
		{
			InitializeComponent();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void cancelBtn_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void okBtn_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}
	}
}
