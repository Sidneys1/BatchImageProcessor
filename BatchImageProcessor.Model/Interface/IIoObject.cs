namespace BatchImageProcessor.Model.Interface
{
	public interface IIoObject
	{
		string Name { get; }
		string Path { get; }
		bool IsFile { get; }
	}
}