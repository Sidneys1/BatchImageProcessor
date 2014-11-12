using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BatchImageProcessor.Controls
{
	/// <summary>
	///     Interaction logic for SplitButton.xaml
	/// </summary>
	public partial class SplitButton
	{
		public SplitButton()
		{
			InitializeComponent();
			DataContext = this;
		}

		public event Action<Object, EventArgs> Click = delegate { };

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			ContextMenu.IsEnabled = true;
			ContextMenu.PlacementTarget = (sender as Button);
			ContextMenu.Placement = PlacementMode.Bottom;
			ContextMenu.IsOpen = true;
		}

		private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Click(this, new EventArgs());
		}
	}
}