﻿using System.Collections.Generic;
using BatchImageProcessor.Model.Interface;
using BatchImageProcessor.Model.Types;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model
{
	// [System.Runtime.Versioning.SupportedOSPlatform("windows")]
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