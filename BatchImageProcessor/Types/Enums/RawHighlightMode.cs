using System.ComponentModel;

namespace BatchImageProcessor.Types.Enums
{
	public enum RawHighlightMode
	{
		Clipped,
		Unclipped,
		Blend,
		[Description("Reconstruct (Whites)")]
		ReconstructHi=3,
		[Description("Reconstruct (Balance)")]
		ReconstructMed =5,
		[Description("Reconstruct (Colors)")]
		ReconstructLo =7
	}
}