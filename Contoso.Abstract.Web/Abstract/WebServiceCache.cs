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
using System.Abstract;
using System.Web;
using WebCacheDependency = System.Web.Caching.CacheDependency;
using WebCacheItemPriority = System.Web.Caching.CacheItemPriority;
using WebCacheItemRemovedCallback = System.Web.Caching.CacheItemRemovedCallback;
using WebCacheItemRemovedReason = System.Web.Caching.CacheItemRemovedReason;
namespace Contoso.Abstract
{
    /// <summary>
    /// IWebServiceCache
    /// </summary>
    public interface IWebServiceCache : IServiceCache { }

    /// <summary>
    /// WebServiceCache
    /// </summary>
    public class WebServiceCache : IWebServiceCache
    {
        private class CacheItemRemovedTranslator
        {
            private CacheItemRemovedCallback _onRemoveCallback;
            public CacheItemRemovedTranslator(CacheItemRemovedCallback onRemoveCallback) { _onRemoveCallback = onRemoveCallback; }
            public void ItemRemovedCallback(string key, object value, WebCacheItemRemovedReason cacheItemRemovedReason) { _onRemoveCallback(key, value); }
        }

        public object this[string name]
        {
            get { return Get(name, null); }
            set { Insert(name, value, null, DateTime.Now.AddMinutes(60), ServiceCache.NoSlidingExpiration, CacheItemPriority.Normal, null, null); }
        }

        public object Add(string name, object value, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object tag)
        {
            // dependency
            WebCacheDependency dependency2;
            WebServiceCacheDependency webDependency;
            if (dependency == null)
                dependency2 = null;
            else if ((webDependency = (dependency as WebServiceCacheDependency)) != null)
                dependency2 = new WebCacheDependency(webDependency.FilePaths, webDependency.CacheTags, webDependency.StartDate);
            else
                dependency2 = new WebCacheDependency(null, dependency.CacheTags, DateTime.MaxValue);
            // item priority
            WebCacheItemPriority cacheItemPriority;
            switch (priority)
            {
                case CacheItemPriority.AboveNormal:
                    cacheItemPriority = WebCacheItemPriority.AboveNormal;
                    break;
                case CacheItemPriority.BelowNormal:
                    cacheItemPriority = WebCacheItemPriority.BelowNormal;
                    break;
                case CacheItemPriority.High:
                    cacheItemPriority = WebCacheItemPriority.High;
                    break;
                case CacheItemPriority.Low:
                    cacheItemPriority = WebCacheItemPriority.Low;
                    break;
                case CacheItemPriority.Normal:
                    cacheItemPriority = WebCacheItemPriority.Normal;
                    break;
                case CacheItemPriority.NotRemovable:
                    cacheItemPriority = WebCacheItemPriority.NotRemovable;
                    break;
                default:
                    cacheItemPriority = WebCacheItemPriority.Default;
                    break;
            }
            // item removed callback
            var cacheItemRemovedCallback = (onRemoveCallback != null ? new WebCacheItemRemovedCallback(new CacheItemRemovedTranslator(onRemoveCallback).ItemRemovedCallback) : null);
            return HttpRuntime.Cache.Add(name, value, dependency2, absoluteExpiration, slidingExpiration, cacheItemPriority, cacheItemRemovedCallback);
        }

        public object Get(string name, object tag)
        {
            return HttpRuntime.Cache.Get(name);
        }

        public object Insert(string name, object value, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object tag)
        {
            // dependency
            WebCacheDependency dependency2;
            WebServiceCacheDependency webDependency;
            if (dependency == null)
                dependency2 = null;
            else if ((webDependency = (dependency as WebServiceCacheDependency)) != null)
                dependency2 = new WebCacheDependency(webDependency.FilePaths, webDependency.CacheTags, webDependency.StartDate);
            else
                dependency2 = new WebCacheDependency(null, dependency.CacheTags, DateTime.MaxValue);
            // item priority
            WebCacheItemPriority cacheItemPriority;
            switch (priority)
            {
                case CacheItemPriority.AboveNormal:
                    cacheItemPriority = WebCacheItemPriority.AboveNormal;
                    break;
                case CacheItemPriority.BelowNormal:
                    cacheItemPriority = WebCacheItemPriority.BelowNormal;
                    break;
                case CacheItemPriority.High:
                    cacheItemPriority = WebCacheItemPriority.High;
                    break;
                case CacheItemPriority.Low:
                    cacheItemPriority = WebCacheItemPriority.Low;
                    break;
                case CacheItemPriority.Normal:
                    cacheItemPriority = WebCacheItemPriority.Normal;
                    break;
                case CacheItemPriority.NotRemovable:
                    cacheItemPriority = WebCacheItemPriority.NotRemovable;
                    break;
                default:
                    cacheItemPriority = WebCacheItemPriority.Default;
                    break;
            }
            // item removed callback
            var cacheItemRemovedCallback = (onRemoveCallback != null ? new WebCacheItemRemovedCallback(new CacheItemRemovedTranslator(onRemoveCallback).ItemRemovedCallback) : null);
            HttpRuntime.Cache.Insert(name, value, dependency2, absoluteExpiration, slidingExpiration, cacheItemPriority, cacheItemRemovedCallback);
            return value;
        }

        public object Remove(string name, object tag)
        {
            return HttpRuntime.Cache.Remove(name);
        }

        public void Touch(string name, object tag)
        {
            Insert(name, string.Empty, null, ServiceCache.NoAbsoluteExpiration, ServiceCache.NoSlidingExpiration, CacheItemPriority.Normal, null, null);
        }
    }
}
