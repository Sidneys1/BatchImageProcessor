// Copyright (c) 2017 Sidneys1
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
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
