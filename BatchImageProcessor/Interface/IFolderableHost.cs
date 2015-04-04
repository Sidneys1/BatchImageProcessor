using System.Collections.ObjectModel;

namespace BatchImageProcessor.Interface
{
	public interface IFolderableHost
	{
		string Name { get; set; }
		ObservableCollection<IFolderable> Files { get; }
	}
}
