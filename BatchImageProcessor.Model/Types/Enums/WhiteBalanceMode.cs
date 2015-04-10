using System.ComponentModel;

namespace BatchImageProcessor.Model.Types.Enums
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