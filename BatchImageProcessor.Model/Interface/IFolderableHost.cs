using System.Collections.ObjectModel;

namespace BatchImageProcessor.Model.Interface
{
	public interface IFolderableHost
	{
		string Name { get; set; }
		ObservableCollection<IFolderable> Files { get; }
	}
}
