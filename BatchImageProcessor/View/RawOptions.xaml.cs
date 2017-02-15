using System.Windows;
using BatchImageProcessor.ViewModel;

namespace BatchImageProcessor.View
{
	/// <summary>
	/// Interaction logic for RawOptions.xaml
	/// </summary>
	public partial class RawOptions
	{
		private FileWrapper _dataContext;

		//public string Info { get; }

		public Visibility ApplyToAllVisible { get; set; } = Visibility.Visible;
		public bool ApplyToAll { get; set; } = false;

		public RawOptions()
		{
			InitializeComponent();
		}


		private void RawOptions_OnLoaded(object sender, RoutedEventArgs e)
		{
			DataContext = _dataContext;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			_dataContext.EndEdit();
			Close();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			_dataContext.CancelEdit();
			Close();
		}

		private void window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_dataContext?.EndEdit();
			_dataContext = (FileWrapper)DataContext;
			_dataContext?.BeginEdit();
		}
	}
}
