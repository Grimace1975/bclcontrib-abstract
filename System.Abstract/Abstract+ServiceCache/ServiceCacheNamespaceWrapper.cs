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
using System.Collections.Generic;
namespace System.Abstract
{
    /// <summary>
    /// ServiceCacheNamespaceWrapper
    /// </summary>
    public class ServiceCacheNamespaceWrapper : IServiceCache
    {
        private IServiceCache _parent;
        private string _namespace;

        public ServiceCacheNamespaceWrapper(IServiceCache parent, string @namespace)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (string.IsNullOrEmpty(@namespace))
                throw new ArgumentNullException("@namespace");
            _parent = parent;
            _namespace = @namespace;
        }

        public object this[string name]
        {
            get { return _parent[_namespace + name]; }
            set { _parent[_namespace + name] = value; }
        }
        public object Add(object tag, string name, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object value) { return _parent.Add(tag, _namespace + name, dependency, absoluteExpiration, slidingExpiration, priority, onRemoveCallback, value); }
        public object Get(object tag, string name) { return _parent.Get(tag, _namespace + name); }
        public object Remove(object tag, string name) { return _parent.Remove(tag, _namespace + name); }
        public object Insert(object tag, string name, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object value) { return _parent.Add(tag, _namespace + name, dependency, absoluteExpiration, slidingExpiration, priority, onRemoveCallback, value); }
        public void Touch(object tag, params string[] names) { _parent.Touch(tag, names);  }
        public ServiceCacheRegistration.IDispatch RegistrationDispatch
        {
            get { return _parent.RegistrationDispatch; }
        }

        public IServiceCache Parent
        {
            get { return _parent; }
        }

        public string Namespace
        {
            get { return _namespace; }
        }
    }
}
