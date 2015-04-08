using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using BatchImageProcessor.Model;
using BatchImageProcessor.Native;
using BatchImageProcessor.Types;
using BatchImageProcessor.View;
using NDesk.Options;
using Xceed.Wpf.Toolkit;
using File = System.IO.File;
using FontFamily = System.Windows.Media.FontFamily;
using OptionSet = NDesk.Options.OptionSet;

namespace BatchImageProcessor
{
	public class EntryPoint : IProgress<ModelProgressUpdate>
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var noShaders = false;
			var noAero = false;

			if (args != null && args.Length > 0)
			{
				var showHelp = false;
				var x = new Types.OptionSet();
				var manifest = string.Empty;

				var fontsize = 12f;
				var fontname = "Calibri";

				#region Option Definitions

				var p = new OptionSet
				{
					{"man=", "A {manifest} file", o => manifest = o},

					{ "rotate=", "A {rotation} transform.\n0=None,\n1-3=Clockwise 90-180-270,\n4=Portrait,\n5=Landscape", (int o) => x.Rotation = (Rotation)o },

					#region Resize Flags

					{ "resize=", "A {resize} transform.\n(None|Smaller|Larger|Exact)", o => {
						ResizeMode r;
						if (Enum.TryParse(o, true, out r)) x.ResizeOptions.ResizeMode = r;
					}},
					{ "rwidth=", "Resize {width}, in pixels.", (int o) => x.ResizeOptions.ResizeWidth = o},
					{ "rheight=", "Resize {height}, in pixels.", (int o) => x.ResizeOptions.ResizeHeight = o},
					{ "a|noaspect", "Disables automatic aspect\nratio matching when resizing.", o => x.ResizeOptions.UseAspectRatio = o == null},

					#endregion

					#region Crop Flags

					{ "c|crop", "Enables cropping.", o => x.EnableCrop = o != null},
					{ "cwidth=", "Crop {width}, in pixels.", (int o) => x.CropOptions.CropWidth = o},
					{ "cheight=", "Crop {height}, in pixels.", (int o) => x.CropOptions.CropHeight = o},
					{ "calign=", "Crop {alignment}.\n0   1   2\n3   4   5\n6   7   8", (int o) => x.CropOptions.CropAlignment = (Alignment) o},

					#endregion

					#region Watermark Flags

					{ "w|watermark", "Enables watermarking.", o => x.EnableWatermark = o != null},
					{ "wtype=", "Watermark {type}.\n(text|image)", o => {
						WatermarkType wt;
						if (Enum.TryParse(o, true, out wt)) x.WatermarkOptions.WatermarkType = wt;
					}},
					{ "wtext=", "Watermark {text}, in quotes.", o => x.WatermarkOptions.WatermarkText = o},
					{ "wfile=", "Watermark image file{path}.", o => x.WatermarkOptions.WatermarkImagePath = o},
					{ "wfont=", "Watermark {font} name.", o => fontname = o},
					{ "wsize=", "Watermark font {size}, in pts.", (float o) => fontsize = o},
					{ "wopac=", "Watermark {opacity}.", (double o) => x.WatermarkOptions.WatermarkOpacity = o},
					{ "wcolor", "Image watermarks in color.", o => x.WatermarkOptions.WatermarkGreyscale = o != null},
					{ "walign=", "Watermark {alignment}.\n0   1   2\n3   4   5\n6   7   8", (int o) => x.WatermarkOptions.WatermarkAlignment = (Alignment) o },

					#endregion

					#region Adjustments Flags

					{ "brightness=", "Brightness {value}.\nE.g. 0.8=80%", (double o) => x.AdjustmentOptions.ColorBrightness = o},
					{ "contrast=", "Contrast {value} %.\nE.g. 0.8=80%", (double o) => x.AdjustmentOptions.ColorContrast = o},
					{ "gamma=", "Gamma {value}.\nMin=0.1, Max=5.0\nE.g. 0.8=80%", (double o) => x.AdjustmentOptions.ColorGamma = o},
					{ "smode=", "Saturation {mode}.\n0=Saturation\n1=Greyscale\n2=Sepia", (int o) => x.AdjustmentOptions.ColorType = (ColorType) o},
					{ "saturation=", "Saturation {value}.\nE.g. 0.8=80%", (double o) => x.AdjustmentOptions.ColorSaturation = o},

					#endregion

					#region Output Flags

					{ "o|output=", "Output directory {path}, in quotes.\nNot specifying this outputs\nto current working directory.", o => x.OutputOptions.OutputPath = o },
					{ "naming=", "Output naming {method}.\n(Original|Numbered|Custom)", o => {
						NameType t;
						if (Enum.TryParse(o, true, out t)) x.OutputOptions.NameOption = t;
					}},
					{ "customname=", "Naming Template, in quotes.\nUse these identifiers:\n{{o}} = Original Filename\n{{w}} = Output Width\n{{h}} = Output Height\nE.g. \"{{o}} - {{w}}x{{h}}\" might result in\n\"DSCF001 - 800x600.jpg\"", o => x.OutputOptions.OutputTemplate = o },
					{ "format=", "Output format, defaults to Jpg.\n(Jpg|Png|Bmp|Gif|Tiff)", o => {
						Format f;
						if (Enum.TryParse(o, true, out f)) x.OutputOptions.OutputFormat = f;
					}},
					{ "jquality=", "Jpeg quality {value}.\nDefaults to 0.95.\nE.g. 0.8 = 80%", (double o) => x.OutputOptions.JpegQuality = o},

					#endregion

					{"?|help", "Show this message and exit", o => showHelp = o != null},

					{"s|noshaders", "Disables shaders in the GUI", o => noShaders = o != null},
					{"e|noaero", "Disables Windows Aero extensions", o => noAero = o != null}
				};

				#endregion

				List<string> extra;
				try
				{
					extra = p.Parse(args);
				}
				catch (OptionException e)
				{
					var b = new StringBuilder("BatchImageProcessor: \r\n");
					b.AppendLine(e.Message);
					b.AppendLine("Try 'BatchImageProcessor --help' for more information.");
					return;
				}

				if (showHelp)
				{
					ShowHelp(p);
					return;
				}

				var files = new List<string>();

				x.WatermarkOptions.WatermarkFont = new Font(fontname, fontsize);

				if ((extra != null && extra.Any()) || !string.IsNullOrEmpty(manifest))
				{
					Debug.Assert(extra != null, "extra != null");
					var badfiles = extra.Where(o => !File.Exists(o)).ToList();
					if (badfiles.Count > 0)
					{
						var b = new StringBuilder("BatchImageProcessor: \r\n");
						b.AppendLine("Bad Filename(s):");
						badfiles.ForEach(o => b.AppendFormat("\t\"{0}\"", o));
						MessageBox.Show(b.ToString());
					}
					if (extra.Any())
						files.AddRange(extra);
					else
					{
						if (!File.Exists(manifest))
							MessageBox.Show("Manifest does not exist!");
						using (var r = File.OpenText(manifest))
						{
							while (!r.EndOfStream)
							{
								var s = r.ReadLine()?.Replace("\"", "");
								if (File.Exists(s))
									files.Add(s);
							}
						}
					}

					if (string.IsNullOrWhiteSpace(x.OutputOptions.OutputPath))
						x.OutputOptions.OutputPath = new DirectoryInfo(".").FullName;

					var mod = new Model.Model(x, files);

					Natives.AllocConsole();
					Console.BufferHeight = Console.WindowWidth;
					Console.BufferWidth = Console.BufferHeight;
					Console.WriteLine();
					Console.CursorVisible = false;
					
					mod.Process(new EntryPoint());

					Console.WriteLine(@"Press ENTER to exit...");
					Console.ReadLine();
					Natives.FreeConsole();

					return;
				}
			}
			var app = new App();
			app.Run(new MainWindow(noShaders, noAero));
		}

		static void ShowHelp(OptionSet p)
		{
			var b = new StringBuilder();
			var s = new StringWriter(b);

			s.WriteLine("Desktop GUI usage: BatchImageProcessor [-s -a]");
			s.WriteLine("\tShows a GUI interface for Batch Image Processor.");

			s.WriteLine();
			s.WriteLine();

			s.WriteLine("Command line usage: BatchImageProcessor [OPTIONS]+ in_files");
			s.WriteLine("\tProcesses in_files with specified options.");
			s.WriteLine("\tOutputs to --output or current directory.");
			s.WriteLine("\tIf no input file is specified, a manifest is required.");
			s.WriteLine();
			s.WriteLine("Options:");
			p.WriteOptionDescriptions(s);

			var m = new MessageBox { FontFamily = new FontFamily("Courier New"), Text = b.ToString(), Width = 600 };
			m.ShowDialog();
		}

		public void Report(ModelProgressUpdate value)
		{
			lock (this)
			{
				var s = $@"Done {value.Done} out of {value.Total}";

				Console.SetCursorPosition((Console.WindowWidth/2) - (s.Length/2), Console.WindowHeight/2);
				Console.WriteLine(s);

				var max = Console.WindowWidth - 2;

				var val = (float) value.Done/value.Total;
				Console.WriteLine(@" " + new string('|', val > 0 ? (int) (max*val) : 1));
				Console.Out.Flush();
			}
		}
	}
}
