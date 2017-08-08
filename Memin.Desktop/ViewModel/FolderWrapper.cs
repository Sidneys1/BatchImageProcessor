﻿// Copyright (c) 2017 Javier Cañon www.javiercanon.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
//using BatchImageProcessor.Annotations;
using BatchImageProcessor.Model;
using BatchImageProcessor.Model.Interface;
// using BatchImageProcessor.Properties;

namespace Memin.Desktop.ViewModel
{
	public class FolderWrapper : INotifyPropertyChanged, IFolderable, IFolderableHost, IEditableObject
	{
		private readonly Folder _folder;

		public FolderWrapper(string fromPath = null, bool recursion = true, bool removable = true)
		{
			_folder = new Folder();

			if (fromPath != null) {
				_folder.Name = Path.GetDirectoryName(fromPath);
				Populate(fromPath, recursion);
			} else
				_folder.Name = Resources.New_Folder_Name;
			Removable = removable;
		}

		public bool Removable { get; private set; }

		public string Name
		{
			get { return _folder.Name; }
			set
			{
				if (_folder.Name?.Equals(value, StringComparison.Ordinal) ?? false) return;
				
				_folder.Name = value;
				PropChanged(nameof(Name));
			}
		}

		public ObservableCollection<IFolderable> Files => _folder.Files;
		
		private void Populate(string path, bool recursive)
		{
			var info = new DirectoryInfo(path);

			Name = info.Name;

			if (recursive)
			{
				var folders = info.GetDirectories();

				foreach (var inf in folders)
				{
					_folder.Files.Add(new FolderWrapper(inf.FullName));
				}
			}

			foreach (var inf in new[] { "*.jpg", "*.jpeg", "*.png" }.Select(str => info.GetFiles(str)).SelectMany(files => files))
			{
				_folder.Files.Add(new FileWrapper(inf.FullName));
			}
		}

		public bool ContainsFile(string p)
		{
			return _folder.ContainsFile(p);
		}

		#region IEditableObject

		private string _backupName;

		public void BeginEdit()
		{
			_backupName = Name;
		}

		public void EndEdit()
		{
			_backupName = null;
			Name = Name.Trim();
			Name = Folder.InvalidCharacters.ToList().Aggregate(Name, (x, y) => x.Replace(y.ToString(), ""));
		}

		public void CancelEdit()
		{
			if (_backupName != null) Name = _backupName;
		}

		#endregion

		public void AddFile(string filepath)
		{
			Files.Add(new FileWrapper(filepath));
		}

        public void AddFile(string filepath, string msgText)
        {
            Files.Add(new FileWrapper(filepath, msgText));
        }

        public event PropertyChangedEventHandler PropertyChanged;

		//[NotifyPropertyChangedInvocator]
		protected virtual void PropChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}