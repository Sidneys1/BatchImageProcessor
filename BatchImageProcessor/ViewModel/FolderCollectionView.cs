using System.Windows.Data;

namespace BatchImageProcessor.ViewModel
{
	public class FolderCollectionView : CollectionViewSource
	{
		public FolderCollectionView()
		{
			Filter += (o, e) => e.Accepted = e.Item is Folder;
		}
	}
}