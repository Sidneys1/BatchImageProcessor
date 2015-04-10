using System.Diagnostics;
using System.Windows;
using BatchImageProcessor.ViewModel;

namespace BatchImageProcessor.View
{
	/// <summary>
	/// Interaction logic for RawOptions.xaml
	/// </summary>
	public partial class RawOptions
	{
		private readonly FileWrapper _dataContext;

		public string Info { get; private set; }

		public Visibility ApplyToAllVisible { get; set; } = Visibility.Visible;
		public bool ApplyToAll { get; set; } = false;

		public RawOptions(string str, string info)
		{
			Info = info;
			InitializeComponent();

			_dataContext = new FileWrapper(str) {RawOptions = new Model.Types.RawOptions()};
		}

		public RawOptions(FileWrapper opts)
		{
			InitializeComponent();
			_dataContext = opts;
			_dataContext.BeginEdit();
			var p = new Process
			{
				StartInfo = new ProcessStartInfo(".\\Exec\\dcraw.exe", $"-i -v \"{opts.Path}\"")
				{
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};
			p.Start();
			var info = p.StandardOutput.ReadToEnd();
			Info = info;
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
	}
}
