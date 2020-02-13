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
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using BatchImageProcessor.Model.Types.Enums;
using BatchImageProcessor.ViewModel;
using static System.Environment;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace BatchImageProcessor.View
{
	/// <summary>
	/// Interaction logic for FileOptions.xaml
	/// </summary>
	public partial class FileOptions : IWin32Window
	{
		private readonly OpenFileDialog _watermarkFileBrowser = new OpenFileDialog
		{
			Title = Properties.Resources.MainWindow__watermarkFileBrowser_Title,
			CheckFileExists = true,
			CheckPathExists = true,
			Filter = Properties.Resources.MainWindow__watermarkFileBrowser_Filter,
			Multiselect = false,
			InitialDirectory = GetFolderPath(SpecialFolder.MyPictures)
		};

		private readonly FontDialog _watermarkFontDlg = new FontDialog
		{
			ShowColor = false
		};

		public FileOptions()
		{
			InitializeComponent();

			Handle = new WindowInteropHelper(this).Handle;
		}

		private void CropBtn_Click(object sender, RoutedEventArgs e)
		{
			var model = DataContext as FileWrapper;

			if (model == null) return;

			if (Equals(sender, CropTopLeftBtn))
				model.Options.CropOptions.CropAlignment = Alignment.Top_Left;
			else if (Equals(sender, CropTopCenterBtn))
				model.Options.CropOptions.CropAlignment = Alignment.Top_Center;
			else if (Equals(sender, CropTopRightBtn))
				model.Options.CropOptions.CropAlignment = Alignment.Top_Right;

			else if (Equals(sender, CropMiddleLeftBtn))
				model.Options.CropOptions.CropAlignment = Alignment.Middle_Left;
			else if (Equals(sender, CropMiddleCenterButton))
				model.Options.CropOptions.CropAlignment = Alignment.Middle_Center;
			else if (Equals(sender, CropMiddleRightBtn))
				model.Options.CropOptions.CropAlignment = Alignment.Middle_Right;

			else if (Equals(sender, CropBottomLeftBtn))
				model.Options.CropOptions.CropAlignment = Alignment.Bottom_Left;
			else if (Equals(sender, CropBottomCenterBtn))
				model.Options.CropOptions.CropAlignment = Alignment.Bottom_Center;
			else if (Equals(sender, CropBottomRightBtn))
				model.Options.CropOptions.CropAlignment = Alignment.Bottom_Right;
		}

		private void WatermarkBtn_Click(object sender, RoutedEventArgs e)
		{
			var model = DataContext as FileWrapper;
			if (model == null) return;
			if (Equals(sender, WatermarkTopLeftBtn))
				model.Options.WatermarkOptions.WatermarkAlignment = Alignment.Top_Left;
			else if (Equals(sender, WatermarkTopCenterBtn))
				model.Options.WatermarkOptions.WatermarkAlignment = Alignment.Top_Center;
			else if (Equals(sender, WatermarkTopRightBtn))
				model.Options.WatermarkOptions.WatermarkAlignment = Alignment.Top_Right;

			else if (Equals(sender, WatermarkMiddleLeftBtn))
				model.Options.WatermarkOptions.WatermarkAlignment = Alignment.Middle_Left;
			else if (Equals(sender, WatermarkMiddleCenterButton))
				model.Options.WatermarkOptions.WatermarkAlignment = Alignment.Middle_Center;
			else if (Equals(sender, WatermarkMiddleRightBtn))
				model.Options.WatermarkOptions.WatermarkAlignment = Alignment.Middle_Right;

			else if (Equals(sender, WatermarkBottopLeftBtn))
				model.Options.WatermarkOptions.WatermarkAlignment = Alignment.Bottom_Left;
			else if (Equals(sender, WatermarkBottomCenterBtn))
				model.Options.WatermarkOptions.WatermarkAlignment = Alignment.Bottom_Center;
			else if (Equals(sender, WatermarkBottomRightBtn))
				model.Options.WatermarkOptions.WatermarkAlignment = Alignment.Bottom_Right;
		}

		private void watermarkFontBtn_Click(object sender, RoutedEventArgs e)
		{
			var model = DataContext as FileWrapper;
			if (model == null) return;
			_watermarkFontDlg.Font = model.Options.WatermarkOptions.WatermarkFont;
			if (_watermarkFontDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				model.Options.WatermarkOptions.WatermarkFont = _watermarkFontDlg.Font;

		}

		private void watermarkImageBtn_Click(object sender, RoutedEventArgs e)
		{
			var model = DataContext as FileWrapper;

			if (model != null && _watermarkFileBrowser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				model.Options.WatermarkOptions.WatermarkImagePath = _watermarkFileBrowser.FileName;
			}
		}

		public IntPtr Handle { get; }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
