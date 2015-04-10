using System.ComponentModel;

namespace BatchImageProcessor.Types.Enums
{
	public enum WhiteBalanceMode
	{
		Default,
		Camera,
		Average,
		[Description("Average (Rectangular Area)")]
		AverageArea,
		Custom
	}
}