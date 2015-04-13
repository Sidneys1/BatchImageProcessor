using System;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model.Types
{
	[Serializable]
	public class RawOptions
	{
		#region Color Options

		public WhiteBalanceMode WhiteBalance { get; set; }
		public int WhiteBalanceLeft { get; set; }
		public int WhiteBalanceTop { get; set; }
		public int WhiteBalanceWidth { get; set; } = 1;
		public int WhiteBalanceHeight { get; set; } = 1;

		public double WhiteBalanceMul0 { get; set; }
		public double WhiteBalanceMul1 { get; set; }
		public double WhiteBalanceMul2 { get; set; }
		public double WhiteBalanceMul3 { get; set; }

		#endregion

		#region Interpolation Options

		public InterpolationMode InterpolationMode { get; set; }
		public bool Cleanup { get; set; } = false;
		public int CleanupPasses { get; set; } = 1;

		#endregion

		#region Output Options

		public bool FixedWhiteLevel { get; set; } = false;
		public double Brighness { get; set; } = 1.0;

		public bool DisableAutoOrient { get; set; } = false;
		public bool NoStretch { get; set; } = false;

		public string Info { get; set; }

		#endregion

		public RawOptions(string info)
		{
			Info = info;
		}
	}
}
