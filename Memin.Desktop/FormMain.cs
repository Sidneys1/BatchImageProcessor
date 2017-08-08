// Copyright (c) 2017 Javier Cañon www.javiercanon.com
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
using Memin.Desktop.ViewModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using BatchImageProcessor.Model.Types;
using System.Drawing;

namespace Memin.Desktop
{
    public partial class FormMain : Form, IDisposable, IProgress<ModelProgressUpdate>
    {

        public ViewModel.ViewModel VModel { get; }
        FolderWrapper folder = new FolderWrapper(removable: true) { Name = Resources.OutputFolder };
        //string[] files = null;
        //string[] folders = null;

        // fonts
        Font fontWM = new Font("Arial", 12), fontMsg = new Font("Arial", 12);

        public FormMain()
        {
            VModel = new ViewModel.ViewModel(this);

            /*
            if (files != null)
                VModel.ImportFiles(files, folder);

            if (folders != null)
            {
                foreach (var s in folders)
                {
                    folder.Files.Add(new FolderWrapper(s));
                }
            }
            */

            InitializeComponent();

            //fix: font.Height and Graphics.MeasureString failing on user machine needs integer
            fontDialogMsg.MinSize = 8;
            fontDialogWatermark.MinSize = 8;

        }

        private void buttonSrcDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK) {
                textBoxSrcDir.Text = folderBrowserDialog1.SelectedPath;
            }

        }

        private void buttonOutputDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                textBoxOutputDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void buttonTxtSource_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                textBoxMsgSource.Text = openFileDialog1.FileName;
            }

        }

        private void buttonImgWatermark_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK) {
                textBoxImgWatermark.Text = openFileDialog1.FileName;
            }
            
        }


        private void buttonFont_Click(object sender, EventArgs e)
        {

            if (fontDialogMsg.ShowDialog(this) == DialogResult.OK)
            {

                fontMsg = new Font(fontDialogMsg.Font.Name, fontDialogMsg.Font.Size,
fontDialogMsg.Font.Style, GraphicsUnit.Pixel);
                labelTextFontExample.Font = new Font(fontDialogMsg.Font.Name, 12, fontDialogMsg.Font.Style, GraphicsUnit.Pixel);
            }

        }

        private void buttonColorMsg_Click(object sender, EventArgs e)
        {

            if (colorDialogMsg.ShowDialog(this) == DialogResult.OK)
            {
                labelTextFontExample.ForeColor = colorDialogMsg.Color;
            }

        }

        private void buttonWmText_Click(object sender, EventArgs e)
        {

            if (fontDialogWatermark.ShowDialog(this) == DialogResult.OK)
            {
                fontWM = new Font(fontDialogWatermark.Font.Name, fontDialogWatermark.Font.Size,
                    fontDialogWatermark.Font.Style, GraphicsUnit.Pixel);
                labelWmTextExample.Font = new Font(fontDialogWatermark.Font.Name, 12, fontDialogWatermark.Font.Style, GraphicsUnit.Pixel);
            }

        }

        private void buttonColorWA_Click(object sender, EventArgs e)
        {
            if (colorDialogWA.ShowDialog(this) == DialogResult.OK)
            {
                labelWmTextExample.ForeColor = colorDialogWA.Color;
            }

        }

        
        private async void StartBtn_Click(object sender, EventArgs e)
        {

            string dir = textBoxSrcDir.Text;
            if (!System.IO.Directory.Exists(dir)) return;

            string outdir = textBoxOutputDir.Text;
            if (!System.IO.Directory.Exists(outdir)) return;

            //if (!VModel.Ready) return;
           
            VModel.Folders.Add(folder);

            VModel.OutputFormat = BatchImageProcessor.Model.Types.Enums.Format.Default;
            VModel.OutputPath = outdir;


            //TODO: other options

            #region messages
            bool bImportMsgs = false;
            var msgFile = textBoxMsgSource.Text;

            if (checkBoxEnableMessages.Checked)
            {

                if (!string.IsNullOrEmpty(msgFile))
                {

                    // import text, one per line
                    if(File.Exists(msgFile)) bImportMsgs = true;
                    

                    VModel.EnableMessage = true;
                    VModel.MessageAlignment = GetPosition(comboBoxMsgPos.SelectedItem == null ? "" : comboBoxMsgPos.SelectedItem.ToString());
                    VModel.MessageOpacity = Convert.ToDouble(numericUpDownMsgOpacity.Value / 100);
                    
                    VModel.MessageText = Resources.WARNING_NO_TEXT_FOUND;// textBoxTextMessage.Text;

                    VModel.MessageFont = new Font(fontMsg.Name, fontMsg.Size,
                    fontMsg.Style, GraphicsUnit.Pixel);

                    VModel.MessageColor = colorDialogMsg.Color.ToArgb();
                    
                }
            }

            #endregion messages


            string[] files =
 Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly)
 .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".gif")).ToArray();

            if (bImportMsgs)
            {
                string[] msgs = File.ReadAllLines(msgFile);
                
                VModel.ImportFiles(files, msgs, folder);
            }
            else
            {
                VModel.ImportFiles(files, null, folder);
            }


            #region watermark
            //>>> WATERMARK

            if (checkBoxEnableWatermark.Checked)
            {

                if (!string.IsNullOrEmpty(textBoxTextWatermark.Text) || !string.IsNullOrEmpty(textBoxImgWatermark.Text))
                {
                    VModel.EnableWatermark = true;
                    VModel.WatermarkAlignment = GetPosition(comboBoxWatermarkPos.SelectedItem == null ? "" : comboBoxWatermarkPos.SelectedItem.ToString());
                    VModel.WatermarkOpacity = Convert.ToDouble(numericUpDownWmOpacity.Value / 100);

                    if (!string.IsNullOrEmpty(textBoxTextWatermark.Text))
                    {
                        VModel.WatermarkType = BatchImageProcessor.Model.Types.Enums.WatermarkType.Text;
                        VModel.WatermarkText = textBoxTextWatermark.Text;

                        VModel.WatermarkFont = new Font(fontWM.Name, fontWM.Size,
                    fontWM.Style, GraphicsUnit.Pixel);

                        VModel.WatermarkColor = colorDialogWA.Color.ToArgb();

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(textBoxImgWatermark.Text))
                        {
                            if (File.Exists(textBoxImgWatermark.Text))
                            {
                                VModel.WatermarkType = BatchImageProcessor.Model.Types.Enums.WatermarkType.Image;
                                VModel.WatermarkImagePath = textBoxImgWatermark.Text;
                            }
                        }

                    }
                }
            }
            //<<< WATERMARK
            #endregion watermark

            #region outputformat

            // default png
            if (comboBoxOutputFormat.SelectedItem == null)
            {
                VModel.OutputFormat = BatchImageProcessor.Model.Types.Enums.Format.Png;
            }
            else
            {

                if (comboBoxOutputFormat.SelectedItem.ToString() == "JPG")
                    VModel.OutputFormat = BatchImageProcessor.Model.Types.Enums.Format.Jpg;
                else if (comboBoxOutputFormat.SelectedItem.ToString() == "PNG")
                    VModel.OutputFormat = BatchImageProcessor.Model.Types.Enums.Format.Png;
                else if (comboBoxOutputFormat.SelectedItem.ToString() == "GIF")
                    VModel.OutputFormat = BatchImageProcessor.Model.Types.Enums.Format.Gif;
                else if (comboBoxOutputFormat.SelectedItem.ToString() == "BMP")
                    VModel.OutputFormat = BatchImageProcessor.Model.Types.Enums.Format.Bmp;
            }

            // resize image
            VModel.ResizeWidth = (int)numericUpDownResizeWidth.Value;
            VModel.ResizeHeight = (int)numericUpDownResizeHeight.Value;
            VModel.UseAspectRatio = checkBoxResizeImg.Checked;

            if (comboBoxResizeMode.SelectedItem == null)
            {
                VModel.ResizeMode = BatchImageProcessor.Model.Types.Enums.ResizeMode.None;
            }
            else
            {
                if (comboBoxResizeMode.SelectedItem.ToString() == "Exact")
                    VModel.ResizeMode = BatchImageProcessor.Model.Types.Enums.ResizeMode.Exact;
                else if (comboBoxResizeMode.SelectedItem.ToString() == "Larger")
                    VModel.ResizeMode = BatchImageProcessor.Model.Types.Enums.ResizeMode.Larger;
                else if (comboBoxResizeMode.SelectedItem.ToString() == "Smaller")
                    VModel.ResizeMode = BatchImageProcessor.Model.Types.Enums.ResizeMode.Smaller;
                else if (comboBoxResizeMode.SelectedItem.ToString() == "None")
                    VModel.ResizeMode = BatchImageProcessor.Model.Types.Enums.ResizeMode.None;
            }
            
            #endregion outputformat


            // _manager.SetOverlayIcon(Properties.Resources.image_export, "Processing");
            progressBar1.Value = 10;
            await VModel.Begin();
            progressBar1.Value = 0;
            //_manager.SetOverlayIcon(null, "");


            //clear folder and files for processed double++
            VModel.Folders.Clear();
            
            MessageBox.Show("Done");
            System.Diagnostics.Process.Start("explorer.exe", outdir);
            
        }

        /*
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            VModel.Cancel();
        }
        */

        public void Report(ModelProgressUpdate value)
        {
            //TODO: fix error thread access to progressbar

            Invoke(new Action(() =>
            {
                progressBar1.Value = value.Done / value.Total;
            }));


            //progressBar1.Value = value.Done / value.Total;
            /*
            Dispatcher.Invoke(() =>
            {
                progressBar1.ProgressValue = (double)value.Done / value.Total;
            });
            */
        }

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/JavierCanon/Social-Office-Memin");
        }

        private void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/JavierCanon/Social-Office-Memin/wiki");
        }

        private void toolStripButtonCustomize_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "http://www.xn--javiercaon-09a.com/p/contacto.html");
        }

        BatchImageProcessor.Model.Types.Enums.Alignment GetPosition(string pos)
        {
            /*
        Top_Left,
        Top_Center,
        Top_Right,
        Middle_Left,
        Middle_Center,
        Middle_Right,
        Bottom_Left,
        Bottom_Center,
        Bottom_Right
             */

            /*
            top left
            top center
            top right
            middle left
            middle center
            middle right
            bottom left
            bottom center
            bottom right
            */


            if (pos == "top left")
                return BatchImageProcessor.Model.Types.Enums.Alignment.Top_Left;
            else if (pos == "top center")
                return BatchImageProcessor.Model.Types.Enums.Alignment.Top_Center;
            else if (pos == "top right")
                return BatchImageProcessor.Model.Types.Enums.Alignment.Top_Right;
            else if (pos == "middle left")
                return BatchImageProcessor.Model.Types.Enums.Alignment.Middle_Left;
            else if (pos == "middle center")
                return BatchImageProcessor.Model.Types.Enums.Alignment.Middle_Center;
            else if (pos == "middle right")
                return BatchImageProcessor.Model.Types.Enums.Alignment.Middle_Right;
            else if (pos == "bottom left")
                return BatchImageProcessor.Model.Types.Enums.Alignment.Bottom_Left;
            else if (pos == "bottom center")
                return BatchImageProcessor.Model.Types.Enums.Alignment.Bottom_Center;
            else if (pos == "bottom right")
                return BatchImageProcessor.Model.Types.Enums.Alignment.Bottom_Right;
            else
                return BatchImageProcessor.Model.Types.Enums.Alignment.Middle_Center;
            
        }
        
    }
}
