#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace System.Abstract
{
	public class FileTouchableCacheItem : ITouchableCacheItem
	{
		private static readonly object _lock = new object();
		private string _directory;
		private Func<object, IEnumerable<string>, CacheItemDependency> _dependencyFactory;

		public FileTouchableCacheItem(Func<object, IEnumerable<string>, CacheItemDependency> dependencyFactory)
		{
			if (dependencyFactory == null)
				throw new ArgumentNullException("dependencyFactory");
			_dependencyFactory = dependencyFactory;
		}
		public FileTouchableCacheItem(Func<object, IEnumerable<string>, CacheItemDependency> dependencyFactory, string directory)
			: this(dependencyFactory) { Directory = directory; }

		public void Touch(object tag, params string[] names)
		{
			if ((names == null) || (names.Length == 0))
				return;
			lock (_lock)
				foreach (var name in names)
				{
					var filePath = GetFilePathForName(name);
					if (filePath == null)
						continue;
					try { WriteBodyForName(name, filePath); }
					catch { };
				}
		}

		public string Directory
		{
			get { return _directory; }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value");
				_directory = (value.EndsWith("\\") ? value : value + "\\");
			}
		}

		public virtual string GetFilePathForName(string name) { return Directory + name + ".txt"; }

		public virtual void WriteBodyForName(string name, string path) { File.WriteAllText(path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n"); }

		public CacheItemDependency MakeDependency(object tag, params string[] names)
		{
			if ((names == null) || (names.Length == 0))
				throw new ArgumentNullException("keys");
			EnsureKeysExist(names);
			return _dependencyFactory(tag, names.Select(key => GetFilePathForName(key)));
		}

		private void EnsureKeysExist(string[] names)
		{
			lock (_lock)
				foreach (var name in names)
				{
					var filePath = GetFilePathForName(name);
					if (filePath == null)
						continue;
					try
					{
						if (!File.Exists(filePath))
							WriteBodyForName(name, filePath);
					}
					catch { };
				}
		}

		public bool CanTouch(object tag, ref string name)
		{
			if ((name == null) || !name.StartsWith("#"))
				return false;
			name = name.Substring(1);
			return true;
		}
	}
}
