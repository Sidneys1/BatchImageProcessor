namespace BatchImageProcessor.Model
{
	public enum Rotation
	{
		Default = 0,
		None,
		Clockwise,
		CounterClockwise,
		UpsideDown,
		Portrait,
		Landscape
	}

	public enum ResizeMode
	{
		Smaller,
		Larger,
		Exact
	}

	public enum WatermarkType
	{
		Text,
		Image
	}

	public enum NameType
	{
		Original,
		Numbered,
		Custom
	}
}