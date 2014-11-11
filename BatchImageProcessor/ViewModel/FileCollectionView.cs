using System.Windows.Data;

namespace BatchImageProcessor.ViewModel
{
	public class FileCollectionView: CollectionViewSource
	{
		public FileCollectionView()
		{
			Filter += (o, e) => e.Accepted = e.Item is FileWrapper;
		}
	}
}
