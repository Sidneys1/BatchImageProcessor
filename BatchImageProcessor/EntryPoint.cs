using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BatchImageProcessor.Types;
using BatchImageProcessor.View;
using NDesk.Options;
using Xceed.Wpf.Toolkit;

namespace BatchImageProcessor
{
	public class EntryPoint
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var noShaders = false;
			var noAero = false;

			if (args != null && args.Length > 0)
			{
				var showHelp = false;
				var x = new OptionStruct();
				var manifest = string.Empty;

				#region Option Definitions

				var p = new OptionSet
				{
					{"man=", "A {manifest} file", o => manifest = o},

					{ "rotate=", "A {rotation} transform.\n0=None,\n1-3=Clockwise 90-180-270,\n4=Portrait,\n5=Landscape", (int o) => x.Rotation = o},

					{"resize=", "A {resize} transform.\n0=None,\n1=Smaller Than,\n2=Larger Than,\n3=Exact", (int o) => x.Size = o},
					{"rwidth=", "Resize {width}, in pixels.", (int o) => x.SizeWidth = o},
					{"rheight=", "Resize {height}, in pixels.", (int o) => x.SizeHeight = o},
					{"a|noaspect", "Disables automatic aspect\nratio matching when resizing.", o => x.SizeSmart = o == null},

					{"c|crop", "Enables cropping.", o => x.Crop = o != null},
					{"cwidth=", "Crop {width}, in pixels.", (int o) => x.CropWidth = o},
					{"cheight=", "Crop {height}, in pixels.", (int o) => x.CropHeight = o},
					{"calign=", "Crop {alignment}.\n0   1   2\n3   4   5\n6   7   8", (int o) => x.CropAlign = o},

					{"w|watermark", "Enables watermarking.", o => x.Watermark = o != null},
					{"wtype=", "Watermark {type}.\n[text|image]", o => x.WatermarkType = o},
					{
						"wparam=", "Watermark {parameter}, in quotes.\nA path for image watermarks.\nText for text watermarks.",
						o => x.WatermarkText = o
					},
					{"wfont=", "Watermark {font} name.", o => x.WatermarkFont = o},
					{"wsize=", "Watermark font {size}, in pts.", (int o) => x.WatermarkFontsize = o},
					{"wopac=", "Watermark {opacity}.", (double o) => x.WatermarkOpac = o},
					{"wcolor", "Image watermarks in color.", o => x.WatermarkGrey = o != null},
					{"walign=", "Watermark {alignment}.\n0   1   2\n3   4   5\n6   7   8", (int o) => x.WatermarkAlign = o},

					{"brightness=", "Brightness {value}.\nE.g. 0.8=80%", (double o) => x.ColorBright = o},
					{"contrast=", "Contrast {value} %.\nE.g. 0.8=80%", (double o) => x.ColorContrast = o},
					{"gamma=", "Gamma {value}.\nMin=0.1, Max=5.0\nE.g. 0.8=80%", (double o) => x.ColorGamma = o},
					{"smode=", "Saturation {mode}.\n0=Saturation\n1=Greyscale\n2=Sepia", (int o) => x.ColorSatMode = o},
					{"saturation=", "Saturation {value}.\nE.g. 0.8=80%", (double o) => x.ColorSat = o},


					{"output=", "Output directory {path}, in quotes.\nNot specifying this outputs\nto current working directory.",o => x.Output = o},
					{"format=", "Output format, defaults to Jpg.\nOptions: Jpg, Png, Bmp, Gif, Tiff", o => x.Format = o},
					{"jquality=", "Jpeg quality {value}.\nDefaults to 0.95.\nE.g. 0.8=80%", (double o) => x.OutJpeg = o},

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

				if ((extra != null && extra.Any()) || !string.IsNullOrEmpty(manifest))
				{
					Debug.Assert(extra != null, "extra != null");
					var badfiles = extra.Where(o => !File.Exists(o)).ToList();
					if (badfiles.Any())
					{
						var b = new StringBuilder("BatchImageProcessor: \r\n");
						b.AppendLine("Bad Filename(s):");
						badfiles.ForEach(o => b.AppendFormat("\t\"{0}\"", o));
						MessageBox.Show(b.ToString());
					}
					if (extra.Any())
						x.Files.AddRange(extra);
					else
					{
						if (!File.Exists(manifest))
							MessageBox.Show("Manifest does not exist!");
						using (var r = File.OpenText(manifest))
						{
							while (!r.EndOfStream)
							{
								var s = r.ReadLine();
								if (File.Exists(s))
									x.Files.Add(s);
							}
						}
					}

					if (string.IsNullOrWhiteSpace(x.Output))
						x.Output = new DirectoryInfo(".").FullName;

					var mod = new Model.Model(x);
					mod.Process();

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
	}
}
