using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using BatchImageProcessor.Model.Interface;

namespace BatchImageProcessor.Model
{
	public class Folder : IFolderable, IFolderableHost
	{
		public static readonly ISet<char> InvalidCharacters = new HashSet<char>(Path.GetInvalidPathChars());
		public string Name { get; set; }
		public ObservableCollection<IFolderable> Files { get; } = new ObservableCollection<IFolderable>();
		
		public bool ContainsFile(string p)
		{
			return Files.Any(o => (o is Folder)
				? string.Equals(((Folder)o).Name, p, StringComparison.Ordinal)
				: string.Equals(((File)o).Name, p, StringComparison.Ordinal)
			);
		}
	}
}
