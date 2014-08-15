using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BatchImageProcessor.Controls
{
	/// <summary>
	/// Interaction logic for SplitButton.xaml
	/// </summary>
	public partial class SplitButton : UserControl
	{
		public event Action<Object, EventArgs> Click = delegate { };

		public SplitButton()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			this.ContextMenu.IsEnabled = true;
			this.ContextMenu.PlacementTarget = (sender as Button);
			this.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			this.ContextMenu.IsOpen = true;

		}

		private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Click(this, new EventArgs());
		}
	}
}
