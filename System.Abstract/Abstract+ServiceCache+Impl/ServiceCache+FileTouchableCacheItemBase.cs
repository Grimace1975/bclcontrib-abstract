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
	public static partial class ServiceCache
	{
		/// <summary>
		/// FileTouchableCacheItemBase
		/// </summary>
		public abstract class FileTouchableCacheItemBase : ITouchableCacheItem
		{
			private static readonly object _lock = new object();
			private IServiceCache _parent;
			private ITouchableCacheItem _base;
			private string _directory;
			private Func<object, IEnumerable<string>, CacheItemDependency> _dependencyFactory;

			public FileTouchableCacheItemBase(IServiceCache parent, ITouchableCacheItem @base)
			{
				_parent = parent;
				_base = @base;
			}

			public virtual void Touch(object tag, string[] names)
			{
				if (string.IsNullOrEmpty(Directory))
				{
					if (_base != null)
						_base.Touch(tag, names);
					return;
				}
				//
				if ((names == null) || (names.Length == 0))
					return;
				var newNames = new List<string>();
				lock (_lock)
					foreach (var name in names)
					{
						var newName = name;
						bool canTouch = CanTouch(tag, ref newName);
						newNames.Add(newName);
						if (!canTouch)
							continue;
						var filePath = GetFilePathForName(newName);
						if (filePath == null)
							continue;
						try { WriteBodyForName(newName, filePath); }
						catch { };
					}
				if (_base != null)
					_base.Touch(tag, newNames.ToArray());
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

			protected virtual string GetFilePathForName(string name) { return Directory + name + ".txt"; }

			protected virtual void WriteBodyForName(string name, string path) { File.WriteAllText(path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n"); }

			public virtual object MakeDependency(object tag, string[] names)
			{
				if (string.IsNullOrEmpty(Directory))
					return (_base != null ? _base.MakeDependency(tag, names) : null);
				//
				if ((names == null) || (names.Length == 0))
					return null;
				string[] newNames;
				EnsureKeysExist(tag, names, out newNames);
				return MakeDependencyInternal(tag, newNames);
			}

			protected abstract object MakeDependencyInternal(object tag, string[] names);

			private void EnsureKeysExist(object tag, string[] names, out string[] newNames)
			{
				var newNames2 = new List<string>();
				lock (_lock)
					foreach (var name in names)
					{
						var newName = name;
						bool canTouch = CanTouch(tag, ref newName);
						newNames2.Add(newName);
						if (!canTouch)
							continue;
						var filePath = GetFilePathForName(newName);
						if (filePath == null)
							continue;
						try
						{
							if (!File.Exists(filePath))
								WriteBodyForName(newName, filePath);
						}
						catch { };
					}
				newNames = newNames2.ToArray();
			}

			public virtual bool CanTouch(object tag, ref string name)
			{
				if ((name == null) || !name.StartsWith("#"))
					return false;
				name = name.Substring(1);
				return true;
			}
		}
	}
}