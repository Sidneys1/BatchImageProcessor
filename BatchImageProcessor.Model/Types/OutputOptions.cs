using System;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model.Types
{
	[Serializable]
	public class OutputOptions
	{
		public string OutputPath { get; set; } = string.Empty;
		public string OutputTemplate { get; set; } = string.Empty;
		public NameType NameOption { get; set; } = NameType.Original;
		public double JpegQuality { get; set; } = 0.95;
		public Format OutputFormat { get; set; } = Format.Jpg;
	}
}