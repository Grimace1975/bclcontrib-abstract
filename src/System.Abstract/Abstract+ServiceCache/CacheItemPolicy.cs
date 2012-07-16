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
namespace System.Abstract
{
    /// <summary>
    /// Provides an object encapulation of adding/removing items to/from a Cache object instance. Provides a delegate
    /// for when the item is removed from Cache.
    /// </summary>
    public class CacheItemPolicy
    {
        /// <summary>
        /// Default
        /// </summary>
        public static readonly CacheItemPolicy Default = new CacheItemPolicy { };
        /// <summary>
        /// Infinite
        /// </summary>
        public static readonly CacheItemPolicy Infinite = new CacheItemPolicy(-1);
        private DateTime _absoluteExpiration;
        private TimeSpan _floatingAbsoluteExpiration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemPolicy"/> class.
        /// </summary>
        public CacheItemPolicy() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemPolicy"/> class.
        /// </summary>
        /// <param name="floatingAbsoluteMinuteTimeout">The floating absolute minute timeout.</param>
        public CacheItemPolicy(int floatingAbsoluteMinuteTimeout)
        {
            if (floatingAbsoluteMinuteTimeout < -1)
                throw new ArgumentOutOfRangeException("floatingMinuteTimeout");
            if (floatingAbsoluteMinuteTimeout >= 0)
                _floatingAbsoluteExpiration = new TimeSpan(0, floatingAbsoluteMinuteTimeout, 0);
            else
                _absoluteExpiration = ServiceCache.InfiniteAbsoluteExpiration;
        }

        /// <summary>
        /// Gets or sets the object instance that contains dependency information that dictates when an item added to cache should be considered invalid.
        /// </summary>
        /// <value>The dependency.</value>
        public CacheItemDependency Dependency { get; set; }

        /// <summary>
        /// Gets or sets the DateTime instance that represent the absolute expiration of the item being added to cache.
        /// </summary>
        /// <value>The absolute expiration.</value>
        public DateTime AbsoluteExpiration
        {
            get
            {
                if (SlidingExpiration != TimeSpan.Zero)
                    return DateTime.MinValue;
                if (_absoluteExpiration == DateTime.MinValue && _floatingAbsoluteExpiration == TimeSpan.Zero)
                    return DateTime.Now.Add(new TimeSpan(1, 0, 0));
                return (_floatingAbsoluteExpiration != TimeSpan.Zero ? DateTime.Now.Add(_floatingAbsoluteExpiration) : _absoluteExpiration);
            }
            set
            {
                if (_floatingAbsoluteExpiration != TimeSpan.Zero)
                    throw new InvalidOperationException("FloatingExpiration already set");
                _absoluteExpiration = value;
            }
        }

        /// <summary>
        /// Gets or sets the DateTime instance that represent the absolute expiration of the item being added to cache.
        /// </summary>
        /// <value>The absolute expiration.</value>
        public TimeSpan FloatingAbsoluteExpiration
        {
            get
            {
                if (SlidingExpiration != TimeSpan.Zero)
                    return TimeSpan.Zero;
                return _floatingAbsoluteExpiration;
            }
            set
            {
                if (_absoluteExpiration != DateTime.MinValue)
                    throw new InvalidOperationException("AbsoluteExpiration already set");
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("value");
                _floatingAbsoluteExpiration = value;
            }
        }

        /// <summary>
        /// Gets or sets the TimeSpan instance that indicate the length of time to use for a sliding or dynamice expiration time.
        /// A sliding expiration is one that is constantly updated, based on the time value provided, whenever the underlying item
        /// having been cached is retrieved from cache.
        /// </summary>
        /// <value>The sliding expiration.</value>
        public TimeSpan SlidingExpiration { get; set; }

        /// <summary>
        /// Gets or sets the value of the CacheItemPriority enumeration associated with this CacheCommand instance.
        /// </summary>
        /// <value>The priority.</value>
        public CacheItemPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the on CacheItemCreatedCallback instance that is invoked whenever the cached item associated with this 
        /// instance of CacheCommand has been created for cache.
        /// </summary>
        /// <value>The on create callback.</value>
        public CacheEntryUpdateCallback UpdateCallback { get; set; }

        /// <summary>
        /// Gets or sets the on CacheItemRemovedCallback instance that is invoked whenever the cached item associated with this 
        /// instance of CacheCommand has been removed from cache.
        /// </summary>
        /// <value>The on remove callback.</value>
        public CacheEntryRemovedCallback RemovedCallback { get; set; }
    }
}
