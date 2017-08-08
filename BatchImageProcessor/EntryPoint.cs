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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BatchImageProcessor.Model.Types;
using BatchImageProcessor.View;
using NDesk.Options;
using Xceed.Wpf.Toolkit;
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
			List<string> extra=null;
			if (args != null && args.Length > 0)
			{
				var showHelp = false;
				var p = new OptionSet
				{
					{"?|help", "Show this message and exit", o => showHelp = o != null},

					{"s|noshaders", "Disables shaders in the GUI", o => noShaders = o != null},
					{"e|noaero", "Disables Windows Aero extensions", o => noAero = o != null}
				};
				
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
			}
			var app = new App();
			app.Run(new MainWindow(noShaders, noAero, extra?.Where(File.Exists).ToArray(), extra?.Where(Directory.Exists).ToArray()));
		}

		static void ShowHelp(OptionSet p)
		{
			var b = new StringBuilder();
			var s = new StringWriter(b);

			s.WriteLine("Desktop GUI usage: BatchImageProcessor [-s -a]");
			s.WriteLine("\tShows a GUI interface for Batch Image Processor.");
			s.WriteLine("\tFor CLI usage, see 'bipcli --help'");
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
