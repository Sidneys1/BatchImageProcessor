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
using System.Collections.Generic;
using BatchImageProcessor.Model.Interface;
using BatchImageProcessor.Model.Types;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model
{
	public class File : IoObject, IFolderable, IFile
	{
		#region Variables

		public OptionSet Options { get; set; } = new OptionSet {Rotation = Rotation.Default};
		public bool Selected { get; set; } = true;

		public RawOptions RawOptions { get; set; } = null;
		public bool IsRaw { get; set; } = false;
		public static readonly ISet<char> InvalidCharacters = new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());

		#endregion

		public new string Name
		{
			get { return base.Name; }
			set { base.Name = value; }
		}

		public bool IsValidName { get; set; }

		public File(string path) : base(path)
		{
			Options.OutputOptions.OutputFormat = Format.Default;
		}

		#region Properties

		
		public int ImageNumber { get; set; }
		public string OutputPath { get; set; }

		#endregion
	}
}