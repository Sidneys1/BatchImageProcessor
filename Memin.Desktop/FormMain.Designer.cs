namespace Memin.Desktop
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAbout = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCustomize = new System.Windows.Forms.ToolStripButton();
            this.groupBoxFiles = new System.Windows.Forms.GroupBox();
            this.buttonOutputDir = new System.Windows.Forms.Button();
            this.textBoxOutputDir = new System.Windows.Forms.TextBox();
            this.buttonSrcDirectory = new System.Windows.Forms.Button();
            this.textBoxSrcDir = new System.Windows.Forms.TextBox();
            this.groupBoxTexts = new System.Windows.Forms.GroupBox();
            this.numericUpDownMsgOpacity = new System.Windows.Forms.NumericUpDown();
            this.labelMsgOpacity = new System.Windows.Forms.Label();
            this.checkBoxEnableMessages = new System.Windows.Forms.CheckBox();
            this.buttonColorMsg = new System.Windows.Forms.Button();
            this.comboBoxMsgPos = new System.Windows.Forms.ComboBox();
            this.labelTextPosition = new System.Windows.Forms.Label();
            this.labelTextFontExample = new System.Windows.Forms.Label();
            this.buttonTextFont = new System.Windows.Forms.Button();
            this.buttonMsgSource = new System.Windows.Forms.Button();
            this.textBoxMsgSource = new System.Windows.Forms.TextBox();
            this.groupBoxWatermark = new System.Windows.Forms.GroupBox();
            this.checkBoxEnableWatermark = new System.Windows.Forms.CheckBox();
            this.buttonColorWA = new System.Windows.Forms.Button();
            this.numericUpDownWmOpacity = new System.Windows.Forms.NumericUpDown();
            this.labelWmOpacity = new System.Windows.Forms.Label();
            this.labelWmTextExample = new System.Windows.Forms.Label();
            this.buttonWmText = new System.Windows.Forms.Button();
            this.textBoxTextWatermark = new System.Windows.Forms.TextBox();
            this.labelTextWatermark = new System.Windows.Forms.Label();
            this.comboBoxWatermarkPos = new System.Windows.Forms.ComboBox();
            this.labelWatermarkPos = new System.Windows.Forms.Label();
            this.buttonImgWatermark = new System.Windows.Forms.Button();
            this.textBoxImgWatermark = new System.Windows.Forms.TextBox();
            this.groupBoxProcess = new System.Windows.Forms.GroupBox();
            this.checkBoxUseAspectRatio = new System.Windows.Forms.CheckBox();
            this.labelResizeMode = new System.Windows.Forms.Label();
            this.comboBoxResizeMode = new System.Windows.Forms.ComboBox();
            this.checkBoxResizeImg = new System.Windows.Forms.CheckBox();
            this.numericUpDownResizeHeight = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownResizeWidth = new System.Windows.Forms.NumericUpDown();
            this.comboBoxOutputFormat = new System.Windows.Forms.ComboBox();
            this.labelOutputFormat = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.fontDialogMsg = new System.Windows.Forms.FontDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fontDialogWatermark = new System.Windows.Forms.FontDialog();
            this.colorDialogMsg = new System.Windows.Forms.ColorDialog();
            this.colorDialogWA = new System.Windows.Forms.ColorDialog();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBoxFiles.SuspendLayout();
            this.groupBoxTexts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMsgOpacity)).BeginInit();
            this.groupBoxWatermark.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWmOpacity)).BeginInit();
            this.groupBoxProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResizeHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResizeWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAbout,
            this.toolStripButtonHelp,
            this.toolStripButtonCustomize});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonAbout
            // 
            this.toolStripButtonAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonAbout, "toolStripButtonAbout");
            this.toolStripButtonAbout.Name = "toolStripButtonAbout";
            this.toolStripButtonAbout.Click += new System.EventHandler(this.toolStripButtonAbout_Click);
            // 
            // toolStripButtonHelp
            // 
            this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonHelp, "toolStripButtonHelp");
            this.toolStripButtonHelp.Name = "toolStripButtonHelp";
            this.toolStripButtonHelp.Click += new System.EventHandler(this.toolStripButtonHelp_Click);
            // 
            // toolStripButtonCustomize
            // 
            this.toolStripButtonCustomize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonCustomize, "toolStripButtonCustomize");
            this.toolStripButtonCustomize.Name = "toolStripButtonCustomize";
            this.toolStripButtonCustomize.Click += new System.EventHandler(this.toolStripButtonCustomize_Click);
            // 
            // groupBoxFiles
            // 
            this.groupBoxFiles.Controls.Add(this.buttonOutputDir);
            this.groupBoxFiles.Controls.Add(this.textBoxOutputDir);
            this.groupBoxFiles.Controls.Add(this.buttonSrcDirectory);
            this.groupBoxFiles.Controls.Add(this.textBoxSrcDir);
            resources.ApplyResources(this.groupBoxFiles, "groupBoxFiles");
            this.groupBoxFiles.Name = "groupBoxFiles";
            this.groupBoxFiles.TabStop = false;
            // 
            // buttonOutputDir
            // 
            resources.ApplyResources(this.buttonOutputDir, "buttonOutputDir");
            this.buttonOutputDir.Name = "buttonOutputDir";
            this.buttonOutputDir.UseVisualStyleBackColor = true;
            this.buttonOutputDir.Click += new System.EventHandler(this.buttonOutputDir_Click);
            // 
            // textBoxOutputDir
            // 
            resources.ApplyResources(this.textBoxOutputDir, "textBoxOutputDir");
            this.textBoxOutputDir.Name = "textBoxOutputDir";
            // 
            // buttonSrcDirectory
            // 
            resources.ApplyResources(this.buttonSrcDirectory, "buttonSrcDirectory");
            this.buttonSrcDirectory.Name = "buttonSrcDirectory";
            this.buttonSrcDirectory.UseVisualStyleBackColor = true;
            this.buttonSrcDirectory.Click += new System.EventHandler(this.buttonSrcDirectory_Click);
            // 
            // textBoxSrcDir
            // 
            resources.ApplyResources(this.textBoxSrcDir, "textBoxSrcDir");
            this.textBoxSrcDir.Name = "textBoxSrcDir";
            // 
            // groupBoxTexts
            // 
            this.groupBoxTexts.Controls.Add(this.numericUpDownMsgOpacity);
            this.groupBoxTexts.Controls.Add(this.labelMsgOpacity);
            this.groupBoxTexts.Controls.Add(this.checkBoxEnableMessages);
            this.groupBoxTexts.Controls.Add(this.buttonColorMsg);
            this.groupBoxTexts.Controls.Add(this.comboBoxMsgPos);
            this.groupBoxTexts.Controls.Add(this.labelTextPosition);
            this.groupBoxTexts.Controls.Add(this.labelTextFontExample);
            this.groupBoxTexts.Controls.Add(this.buttonTextFont);
            this.groupBoxTexts.Controls.Add(this.buttonMsgSource);
            this.groupBoxTexts.Controls.Add(this.textBoxMsgSource);
            resources.ApplyResources(this.groupBoxTexts, "groupBoxTexts");
            this.groupBoxTexts.Name = "groupBoxTexts";
            this.groupBoxTexts.TabStop = false;
            // 
            // numericUpDownMsgOpacity
            // 
            this.numericUpDownMsgOpacity.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.numericUpDownMsgOpacity, "numericUpDownMsgOpacity");
            this.numericUpDownMsgOpacity.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownMsgOpacity.Name = "numericUpDownMsgOpacity";
            this.numericUpDownMsgOpacity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // labelMsgOpacity
            // 
            resources.ApplyResources(this.labelMsgOpacity, "labelMsgOpacity");
            this.labelMsgOpacity.Name = "labelMsgOpacity";
            // 
            // checkBoxEnableMessages
            // 
            resources.ApplyResources(this.checkBoxEnableMessages, "checkBoxEnableMessages");
            this.checkBoxEnableMessages.Name = "checkBoxEnableMessages";
            this.checkBoxEnableMessages.UseVisualStyleBackColor = true;
            // 
            // buttonColorMsg
            // 
            resources.ApplyResources(this.buttonColorMsg, "buttonColorMsg");
            this.buttonColorMsg.Name = "buttonColorMsg";
            this.buttonColorMsg.UseVisualStyleBackColor = true;
            this.buttonColorMsg.Click += new System.EventHandler(this.buttonColorMsg_Click);
            // 
            // comboBoxMsgPos
            // 
            this.comboBoxMsgPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMsgPos.FormattingEnabled = true;
            this.comboBoxMsgPos.Items.AddRange(new object[] {
            resources.GetString("comboBoxMsgPos.Items"),
            resources.GetString("comboBoxMsgPos.Items1"),
            resources.GetString("comboBoxMsgPos.Items2"),
            resources.GetString("comboBoxMsgPos.Items3"),
            resources.GetString("comboBoxMsgPos.Items4"),
            resources.GetString("comboBoxMsgPos.Items5"),
            resources.GetString("comboBoxMsgPos.Items6"),
            resources.GetString("comboBoxMsgPos.Items7"),
            resources.GetString("comboBoxMsgPos.Items8")});
            resources.ApplyResources(this.comboBoxMsgPos, "comboBoxMsgPos");
            this.comboBoxMsgPos.Name = "comboBoxMsgPos";
            // 
            // labelTextPosition
            // 
            resources.ApplyResources(this.labelTextPosition, "labelTextPosition");
            this.labelTextPosition.Name = "labelTextPosition";
            // 
            // labelTextFontExample
            // 
            this.labelTextFontExample.AutoEllipsis = true;
            resources.ApplyResources(this.labelTextFontExample, "labelTextFontExample");
            this.labelTextFontExample.Name = "labelTextFontExample";
            // 
            // buttonTextFont
            // 
            resources.ApplyResources(this.buttonTextFont, "buttonTextFont");
            this.buttonTextFont.Name = "buttonTextFont";
            this.buttonTextFont.UseVisualStyleBackColor = true;
            this.buttonTextFont.Click += new System.EventHandler(this.buttonFont_Click);
            // 
            // buttonMsgSource
            // 
            resources.ApplyResources(this.buttonMsgSource, "buttonMsgSource");
            this.buttonMsgSource.Name = "buttonMsgSource";
            this.buttonMsgSource.UseVisualStyleBackColor = true;
            this.buttonMsgSource.Click += new System.EventHandler(this.buttonTxtSource_Click);
            // 
            // textBoxMsgSource
            // 
            resources.ApplyResources(this.textBoxMsgSource, "textBoxMsgSource");
            this.textBoxMsgSource.Name = "textBoxMsgSource";
            // 
            // groupBoxWatermark
            // 
            this.groupBoxWatermark.Controls.Add(this.checkBoxEnableWatermark);
            this.groupBoxWatermark.Controls.Add(this.buttonColorWA);
            this.groupBoxWatermark.Controls.Add(this.numericUpDownWmOpacity);
            this.groupBoxWatermark.Controls.Add(this.labelWmOpacity);
            this.groupBoxWatermark.Controls.Add(this.labelWmTextExample);
            this.groupBoxWatermark.Controls.Add(this.buttonWmText);
            this.groupBoxWatermark.Controls.Add(this.textBoxTextWatermark);
            this.groupBoxWatermark.Controls.Add(this.labelTextWatermark);
            this.groupBoxWatermark.Controls.Add(this.comboBoxWatermarkPos);
            this.groupBoxWatermark.Controls.Add(this.labelWatermarkPos);
            this.groupBoxWatermark.Controls.Add(this.buttonImgWatermark);
            this.groupBoxWatermark.Controls.Add(this.textBoxImgWatermark);
            resources.ApplyResources(this.groupBoxWatermark, "groupBoxWatermark");
            this.groupBoxWatermark.Name = "groupBoxWatermark";
            this.groupBoxWatermark.TabStop = false;
            // 
            // checkBoxEnableWatermark
            // 
            resources.ApplyResources(this.checkBoxEnableWatermark, "checkBoxEnableWatermark");
            this.checkBoxEnableWatermark.Name = "checkBoxEnableWatermark";
            this.checkBoxEnableWatermark.UseVisualStyleBackColor = true;
            // 
            // buttonColorWA
            // 
            resources.ApplyResources(this.buttonColorWA, "buttonColorWA");
            this.buttonColorWA.Name = "buttonColorWA";
            this.buttonColorWA.UseVisualStyleBackColor = true;
            this.buttonColorWA.Click += new System.EventHandler(this.buttonColorWA_Click);
            // 
            // numericUpDownWmOpacity
            // 
            this.numericUpDownWmOpacity.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.numericUpDownWmOpacity, "numericUpDownWmOpacity");
            this.numericUpDownWmOpacity.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownWmOpacity.Name = "numericUpDownWmOpacity";
            this.numericUpDownWmOpacity.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // labelWmOpacity
            // 
            resources.ApplyResources(this.labelWmOpacity, "labelWmOpacity");
            this.labelWmOpacity.Name = "labelWmOpacity";
            // 
            // labelWmTextExample
            // 
            this.labelWmTextExample.AutoEllipsis = true;
            resources.ApplyResources(this.labelWmTextExample, "labelWmTextExample");
            this.labelWmTextExample.Name = "labelWmTextExample";
            // 
            // buttonWmText
            // 
            resources.ApplyResources(this.buttonWmText, "buttonWmText");
            this.buttonWmText.Name = "buttonWmText";
            this.buttonWmText.UseVisualStyleBackColor = true;
            this.buttonWmText.Click += new System.EventHandler(this.buttonWmText_Click);
            // 
            // textBoxTextWatermark
            // 
            resources.ApplyResources(this.textBoxTextWatermark, "textBoxTextWatermark");
            this.textBoxTextWatermark.Name = "textBoxTextWatermark";
            // 
            // labelTextWatermark
            // 
            resources.ApplyResources(this.labelTextWatermark, "labelTextWatermark");
            this.labelTextWatermark.Name = "labelTextWatermark";
            // 
            // comboBoxWatermarkPos
            // 
            this.comboBoxWatermarkPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWatermarkPos.FormattingEnabled = true;
            this.comboBoxWatermarkPos.Items.AddRange(new object[] {
            resources.GetString("comboBoxWatermarkPos.Items"),
            resources.GetString("comboBoxWatermarkPos.Items1"),
            resources.GetString("comboBoxWatermarkPos.Items2"),
            resources.GetString("comboBoxWatermarkPos.Items3"),
            resources.GetString("comboBoxWatermarkPos.Items4"),
            resources.GetString("comboBoxWatermarkPos.Items5"),
            resources.GetString("comboBoxWatermarkPos.Items6"),
            resources.GetString("comboBoxWatermarkPos.Items7"),
            resources.GetString("comboBoxWatermarkPos.Items8")});
            resources.ApplyResources(this.comboBoxWatermarkPos, "comboBoxWatermarkPos");
            this.comboBoxWatermarkPos.Name = "comboBoxWatermarkPos";
            // 
            // labelWatermarkPos
            // 
            resources.ApplyResources(this.labelWatermarkPos, "labelWatermarkPos");
            this.labelWatermarkPos.Name = "labelWatermarkPos";
            // 
            // buttonImgWatermark
            // 
            resources.ApplyResources(this.buttonImgWatermark, "buttonImgWatermark");
            this.buttonImgWatermark.Name = "buttonImgWatermark";
            this.buttonImgWatermark.UseVisualStyleBackColor = true;
            this.buttonImgWatermark.Click += new System.EventHandler(this.buttonImgWatermark_Click);
            // 
            // textBoxImgWatermark
            // 
            resources.ApplyResources(this.textBoxImgWatermark, "textBoxImgWatermark");
            this.textBoxImgWatermark.Name = "textBoxImgWatermark";
            // 
            // groupBoxProcess
            // 
            this.groupBoxProcess.Controls.Add(this.checkBoxUseAspectRatio);
            this.groupBoxProcess.Controls.Add(this.labelResizeMode);
            this.groupBoxProcess.Controls.Add(this.comboBoxResizeMode);
            this.groupBoxProcess.Controls.Add(this.checkBoxResizeImg);
            this.groupBoxProcess.Controls.Add(this.numericUpDownResizeHeight);
            this.groupBoxProcess.Controls.Add(this.numericUpDownResizeWidth);
            this.groupBoxProcess.Controls.Add(this.comboBoxOutputFormat);
            this.groupBoxProcess.Controls.Add(this.labelOutputFormat);
            this.groupBoxProcess.Controls.Add(this.buttonRun);
            this.groupBoxProcess.Controls.Add(this.progressBar1);
            resources.ApplyResources(this.groupBoxProcess, "groupBoxProcess");
            this.groupBoxProcess.Name = "groupBoxProcess";
            this.groupBoxProcess.TabStop = false;
            // 
            // checkBoxUseAspectRatio
            // 
            resources.ApplyResources(this.checkBoxUseAspectRatio, "checkBoxUseAspectRatio");
            this.checkBoxUseAspectRatio.Checked = true;
            this.checkBoxUseAspectRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUseAspectRatio.Name = "checkBoxUseAspectRatio";
            this.checkBoxUseAspectRatio.UseVisualStyleBackColor = true;
            // 
            // labelResizeMode
            // 
            resources.ApplyResources(this.labelResizeMode, "labelResizeMode");
            this.labelResizeMode.Name = "labelResizeMode";
            // 
            // comboBoxResizeMode
            // 
            this.comboBoxResizeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxResizeMode.FormattingEnabled = true;
            this.comboBoxResizeMode.Items.AddRange(new object[] {
            resources.GetString("comboBoxResizeMode.Items"),
            resources.GetString("comboBoxResizeMode.Items1"),
            resources.GetString("comboBoxResizeMode.Items2"),
            resources.GetString("comboBoxResizeMode.Items3")});
            resources.ApplyResources(this.comboBoxResizeMode, "comboBoxResizeMode");
            this.comboBoxResizeMode.Name = "comboBoxResizeMode";
            // 
            // checkBoxResizeImg
            // 
            resources.ApplyResources(this.checkBoxResizeImg, "checkBoxResizeImg");
            this.checkBoxResizeImg.Name = "checkBoxResizeImg";
            this.checkBoxResizeImg.UseVisualStyleBackColor = true;
            // 
            // numericUpDownResizeHeight
            // 
            resources.ApplyResources(this.numericUpDownResizeHeight, "numericUpDownResizeHeight");
            this.numericUpDownResizeHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownResizeHeight.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownResizeHeight.Name = "numericUpDownResizeHeight";
            this.numericUpDownResizeHeight.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // numericUpDownResizeWidth
            // 
            resources.ApplyResources(this.numericUpDownResizeWidth, "numericUpDownResizeWidth");
            this.numericUpDownResizeWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownResizeWidth.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownResizeWidth.Name = "numericUpDownResizeWidth";
            this.numericUpDownResizeWidth.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // comboBoxOutputFormat
            // 
            this.comboBoxOutputFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOutputFormat.FormattingEnabled = true;
            this.comboBoxOutputFormat.Items.AddRange(new object[] {
            resources.GetString("comboBoxOutputFormat.Items"),
            resources.GetString("comboBoxOutputFormat.Items1"),
            resources.GetString("comboBoxOutputFormat.Items2"),
            resources.GetString("comboBoxOutputFormat.Items3")});
            resources.ApplyResources(this.comboBoxOutputFormat, "comboBoxOutputFormat");
            this.comboBoxOutputFormat.Name = "comboBoxOutputFormat";
            // 
            // labelOutputFormat
            // 
            resources.ApplyResources(this.labelOutputFormat, "labelOutputFormat");
            this.labelOutputFormat.Name = "labelOutputFormat";
            // 
            // buttonRun
            // 
            resources.ApplyResources(this.buttonRun, "buttonRun");
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // fontDialogMsg
            // 
            this.fontDialogMsg.AllowVectorFonts = false;
            this.fontDialogMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontDialogMsg.FontMustExist = true;
            this.fontDialogMsg.ShowHelp = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.ShowReadOnly = true;
            // 
            // fontDialogWatermark
            // 
            this.fontDialogWatermark.AllowVectorFonts = false;
            this.fontDialogWatermark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontDialogWatermark.FontMustExist = true;
            this.fontDialogWatermark.ShowHelp = true;
            // 
            // colorDialogMsg
            // 
            this.colorDialogMsg.AnyColor = true;
            this.colorDialogMsg.FullOpen = true;
            // 
            // colorDialogWA
            // 
            this.colorDialogWA.AnyColor = true;
            this.colorDialogWA.FullOpen = true;
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.groupBoxProcess);
            this.Controls.Add(this.groupBoxWatermark);
            this.Controls.Add(this.groupBoxTexts);
            this.Controls.Add(this.groupBoxFiles);
            this.Name = "FormMain";
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBoxFiles.ResumeLayout(false);
            this.groupBoxFiles.PerformLayout();
            this.groupBoxTexts.ResumeLayout(false);
            this.groupBoxTexts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMsgOpacity)).EndInit();
            this.groupBoxWatermark.ResumeLayout(false);
            this.groupBoxWatermark.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWmOpacity)).EndInit();
            this.groupBoxProcess.ResumeLayout(false);
            this.groupBoxProcess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResizeHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResizeWidth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxFiles;
        private System.Windows.Forms.GroupBox groupBoxTexts;
        private System.Windows.Forms.GroupBox groupBoxWatermark;
        private System.Windows.Forms.GroupBox groupBoxProcess;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FontDialog fontDialogMsg;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonOutputDir;
        private System.Windows.Forms.TextBox textBoxOutputDir;
        private System.Windows.Forms.Button buttonSrcDirectory;
        private System.Windows.Forms.TextBox textBoxSrcDir;
        private System.Windows.Forms.Button buttonMsgSource;
        private System.Windows.Forms.TextBox textBoxMsgSource;
        private System.Windows.Forms.Button buttonImgWatermark;
        private System.Windows.Forms.TextBox textBoxImgWatermark;
        private System.Windows.Forms.Label labelTextFontExample;
        private System.Windows.Forms.Button buttonTextFont;
        private System.Windows.Forms.ComboBox comboBoxMsgPos;
        private System.Windows.Forms.Label labelTextPosition;
        private System.Windows.Forms.ComboBox comboBoxWatermarkPos;
        private System.Windows.Forms.Label labelWatermarkPos;
        private System.Windows.Forms.TextBox textBoxTextWatermark;
        private System.Windows.Forms.Label labelTextWatermark;
        private System.Windows.Forms.FontDialog fontDialogWatermark;
        private System.Windows.Forms.Label labelWmTextExample;
        private System.Windows.Forms.Button buttonWmText;
        private System.Windows.Forms.Label labelWmOpacity;
        private System.Windows.Forms.NumericUpDown numericUpDownWmOpacity;
        private System.Windows.Forms.ComboBox comboBoxOutputFormat;
        private System.Windows.Forms.Label labelOutputFormat;
        public System.Windows.Forms.ColorDialog colorDialogMsg;
        public System.Windows.Forms.ColorDialog colorDialogWA;
        private System.Windows.Forms.Button buttonColorWA;
        private System.Windows.Forms.Button buttonColorMsg;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonHelp;
        private System.Windows.Forms.CheckBox checkBoxResizeImg;
        private System.Windows.Forms.NumericUpDown numericUpDownResizeHeight;
        private System.Windows.Forms.NumericUpDown numericUpDownResizeWidth;
        private System.Windows.Forms.ComboBox comboBoxResizeMode;
        private System.Windows.Forms.Label labelResizeMode;
        private System.Windows.Forms.CheckBox checkBoxUseAspectRatio;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbout;
        private System.Windows.Forms.ToolStripButton toolStripButtonCustomize;
        private System.Windows.Forms.CheckBox checkBoxEnableWatermark;
        private System.Windows.Forms.CheckBox checkBoxEnableMessages;
        private System.Windows.Forms.NumericUpDown numericUpDownMsgOpacity;
        private System.Windows.Forms.Label labelMsgOpacity;
    }
}

