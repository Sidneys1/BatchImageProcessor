using System.Windows.Data;

namespace BatchImageProcessor.ViewModel
{
#pragma warning disable CA1052
    public class FileCollectionView : CollectionViewSource
#pragma warning restore CA1052
    {
        public FileCollectionView()
        {
            Filter += (o, e) => e.Accepted = e.Item is FileWrapper;
        }
    }
}