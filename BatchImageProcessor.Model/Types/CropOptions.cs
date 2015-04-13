using System;
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model.Types
{
	[Serializable]
	public class CropOptions
	{
		public Alignment CropAlignment { get; set; } = Alignment.Middle_Center;
		public int CropWidth { get; set; } = 800;
		public int CropHeight { get; set; } = 600;
	}
}