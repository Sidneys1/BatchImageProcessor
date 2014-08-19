using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BatchImageProcessor.ViewModel
{
	public class FileCollectionView: CollectionViewSource
	{
		public FileCollectionView():base()
		{
			this.Filter += (o, e) => e.Accepted = e.Item is FileWrapper;
		}
	}
}
