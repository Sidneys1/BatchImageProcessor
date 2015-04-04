using System.Windows.Data;
using BatchImageProcessor.Model;

namespace BatchImageProcessor.ViewModel
{
    public class FolderCollectionView : CollectionViewSource
    {
        public FolderCollectionView()
        {
            Filter += (o, e) => e.Accepted = e.Item is FolderWrapper;
        }
    }
}