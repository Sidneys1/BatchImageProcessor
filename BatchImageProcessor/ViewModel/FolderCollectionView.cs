using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BatchImageProcessor.ViewModel
{
	public class FolderCollectionView: CollectionViewSource
	{
		public FolderCollectionView():base()
		{
			this.Filter += (o, e) => e.Accepted = e.Item is Folder;
		}
	}
}
